set PASSWORD=%1

mkdir temp
aspire publish -o temp 


docker save -o temp/cardpecker-api.tar cardpecker-api:latest

scp -r ./temp/* joschi@media.joschiz.pro:~/projects/cardpecker-api

rmdir -r ./temp

ssh joschi@media.joschiz.pro  ^
cd ~/projects/cardpecker-api && ^
sudo docker load -i cardpecker-api.tar && ^
sudo docker compose up -d