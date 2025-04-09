   # Use the official .NET runtime image as the base image
   FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
   WORKDIR /app
   EXPOSE 8080

   # Use the .NET SDK image for building the app
   FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
   WORKDIR /src
   COPY . .
   RUN dotnet restore "BlazorConnections.csproj"
   RUN dotnet publish "BlazorConnections.csproj" -c Release -o /app/publish

   # Final image with the app ready to run
   FROM base AS final
   WORKDIR /app
   COPY --from=build /app/publish .
   ENTRYPOINT ["dotnet", "BlazorConnections.dll"]