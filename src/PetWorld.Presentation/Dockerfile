FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

COPY ["src/PetWorld.Domain/PetWorld.Domain.csproj", "PetWorld.Domain/"]
COPY ["src/PetWorld.Application/PetWorld.Application.csproj", "PetWorld.Application/"]
COPY ["src/PetWorld.Infrastructure/PetWorld.Infrastructure.csproj", "PetWorld.Infrastructure/"]
COPY ["src/PetWorld.Presentation/PetWorld.Presentation.csproj", "PetWorld.Presentation/"]

RUN dotnet restore "PetWorld.Presentation/PetWorld.Presentation.csproj"

COPY src/ .
WORKDIR "/src/PetWorld.Presentation"
RUN dotnet build "PetWorld.Presentation.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "PetWorld.Presentation.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "PetWorld.Presentation.dll"]
