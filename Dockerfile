FROM microsoft/dotnet
WORKDIR /app
EXPOSE 80
ADD API-Server/obj/Docker/publish /app
RUN cp /usr/share/zoneinfo/Asia/Shanghai /etc/localtime
ENTRYPOINT ["dotnet", "API_Server.dll"]
