FROM mcr.microsoft.com/dotnet/sdk:7.0 as build
WORKDIR /app
EXPOSE 80

#copy a;; .csproject files and restore as disctinct layers. Use of the same COPY command

#for every dockerfile in the project to take advantage of docker caching

COPY Licyt.sln Licyt.sln 
COPY src/AuctionService/AuctionService.csproj src/AuctionService/AuctionService.csproj
COPY src/Contracts/Contracts.csproj src/Contracts/Contracts.csproj
COPY src/SearchService/SearchService.csproj src/SearchService/SearchService.csproj
COPY src/GetwayService/GetwayService.csproj src/GetwayService/GetwayService.csproj 
COPY src/BiddingService/BiddingService.csproj src/BiddingService/BiddingService.csproj 
COPY src/NotificationService/NotificationService.csproj src/NotificationService/NotificationService.csproj
COPY src/IdentityService/IdentityService.csproj src/IdentityService/IdentityService.csproj 

# Restore package deps
RUN dotnet restore Licyt.sln

# Copy the app folders over
COPY src/IdentityService src/IdentityService
WORKDIR /app/src/IdentityService
RUN dotnet publish -c Release -o /app/src/out

#Build runtime image

FROM mcr.microsoft.com/dotnet/aspnet:7.0
WORKDIR /app
COPY --from=build /app/src/out .
ENTRYPOINT [ "dotnet" , "IdentityService.dll" ]