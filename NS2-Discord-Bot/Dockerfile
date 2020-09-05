FROM microsoft/dotnet:2.2-sdk AS build
WORKDIR /app

COPY NS2-Discord-Bot.csproj .
RUN dotnet restore NS2-Discord-Bot.csproj

COPY . .
RUN dotnet publish -c Release -o out

FROM microsoft/dotnet:2.2-aspnetcore-runtime AS runtime
WORKDIR /app
COPY --from=build /app/out ./

ENTRYPOINT ["dotnet", "NS2-Discord-Bot.dll"]