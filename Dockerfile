FROM microsoft/dotnet
WORKDIR /app
EXPOSE 80
ADD API-SERVER/obj/Docker/publish /app
RUN cp /usr/share/zoneinfo/Asia/Shanghai /etc/localtime
ENTRYPOINT ["dotnet", "API_Server.dll"]
