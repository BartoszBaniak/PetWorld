FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy solution and project files
COPY PetWorld.slnx ./
COPY PetWorld.Domain/PetWorld.Domain.csproj ./PetWorld.Domain/
COPY PetWorld.Application/PetWorld.Application.csproj ./PetWorld.Application/
COPY PetWorld.Infrastructure/PetWorld.Infrastructure.csproj ./PetWorld.Infrastructure/
COPY PetWorld/PetWorld.csproj ./PetWorld/

# Restore dependencies
RUN dotnet restore PetWorld/PetWorld.csproj

# Copy everything else
COPY PetWorld.Domain/ ./PetWorld.Domain/
COPY PetWorld.Application/ ./PetWorld.Application/
COPY PetWorld.Infrastructure/ ./PetWorld.Infrastructure/
COPY PetWorld/ ./PetWorld/

# Build and publish
WORKDIR /src/PetWorld
RUN dotnet publish -c Release -o /app/publish

# Runtime image
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .

EXPOSE 5000

ENV ASPNETCORE_URLS=http://+:5000

ENTRYPOINT ["dotnet", "PetWorld.dll"]
