set PASSWORD=%1

mkdir temp
aspire publish -o temp 


docker save -o temp/cardpecker-api.tar cardpecker-api:latest

scp -r ./temp/* joschi@media.joschiz.pro:~/projects/cardpecker-api

rmdir -r ./temp

ECHO ENTER THE FOLLOWING COMMANDS INTO THE SSH SESSION
ECHO cd ~/projects/cardpecker-api
ECHO sudo docker load -i cardpecker-api.tar
ECHO sudo docker compose up -d

ssh joschi@media.joschiz.pro
