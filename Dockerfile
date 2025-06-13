FROM registry.redhat.io/ubi8/ubi-minimal:8.9 AS certificate

RUN microdnf -y install curl
RUN microdnf -y install openssl

RUN curl -o ./BXS-ROOTBancorpSouth\ Root\ Certificate\ Authority.crt http://certs.bank.ad.bxs.com/pki/BXS-ROOTBancorpSouth%20Root%20Certificate%20Authority.crt
RUN openssl x509 -inform DER -in BXS-ROOTBancorpSouth\ Root\ Certificate\ Authority.crt -outform PEM -out /tmp/BancorpSouthCertificate.crt


FROM registry.redhat.io/rhel8/dotnet-80:8.0 AS build-env

USER 0

RUN mkdir FoodOrdering

RUN mkdir SSLCerts
COPY --from=certificate /tmp/BancorpSouthCertificate.crt ./SSLCerts
ENV SSL_CERT_DIR=./SSLCerts

COPY FoodOrdering/*.csproj ./FoodOrdering

COPY SignalRPOC%20Solution.sln SignalRPOC%20Solution.sln

COPY ./nuget.config ./
ENV DOTNET_RESTORE_CONFIGFILE=./nuget.config
ENV DOTNET_NUGET_SIGNATURE_VERIFICATION=false
RUN dotnet restore

COPY FoodOrdering ./FoodOrdering

RUN rm ./FoodOrdering/appsettings.Development.json

# Remove source files after assemble, not needed to run
ENV DOTNET_RM_SRC=true

ENV DOTNET_STARTUP_PROJECT=FoodOrdering/FoodOrdering.csproj

RUN /usr/libexec/s2i/assemble

RUN chown -R 1001:0 /opt/app-root && fix-permissions /opt/app-root

USER 1001

# build runtime image
FROM registry.redhat.io/rhel8/dotnet-80-runtime:8.0

USER 0

COPY --from=certificate /tmp/BancorpSouthCertificate.crt /etc/pki/ca-trust/source/anchors/
RUN update-ca-trust

RUN microdnf install -y krb5-workstation krb5-libs

COPY --from=build-env /opt/app-root /opt/app-root

RUN chown -R 1001:0 /opt/app-root && fix-permissions /opt/app-root

# Run container by default as user with id 1001 (default)
USER 1001


ENTRYPOINT ["/usr/libexec/s2i/run"]