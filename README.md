Deployment steps
----------------

On the server:
- Set up .NET Core
- Set up a HTTP server with HTTPS support (like Nginx + a cert from Let's Encrypt)
- Point a DNS record to it
- Replace `your.hostname` in all configuration files with your server hostname, assuming identity server is on port 5000 and the API is on port 5001. (just do a global find-and-replace over the entire solution and you're ready to go)
- `dotnet publish`
- copy the files to the server (the ones in the `publish` directories for both identity server and the API)
- copy the initial databases from the project directories (called `mydb.db`) to the target directories
- Configure your Nginx to reverse proxy over HTTP from 0.0.0.0:5000 to localhost:5500 and 0.0.0.0:5001 to localhost:5501
- go to the respective directories and run `dotnet Api.dll` and `dotnet IdentityServerWithAspNetIdentity.dll` respectively.

Sample Nginx configuration
--------------------------

```
server {
        listen 5000 default_server ssl;
        listen [::]:5000 default_server ssl;

        ssl_certificate /etc/letsencrypt/live/your.hostname/fullchain.pem;
        ssl_certificate_key /etc/letsencrypt/live/your.hostname/privkey.pem;

        location / {
                proxy_set_header Host $host;
                proxy_set_header X-Real-IP $remote_addr;
                proxy_pass http://127.0.0.1:5500;
        }

        server_name your.hostname;
}

server {
        listen 5001 default_server ssl;
        listen [::]:5001 default_server ssl;

        ssl_certificate /etc/letsencrypt/live/your.hostname/fullchain.pem;
        ssl_certificate_key /etc/letsencrypt/live/your.hostname/privkey.pem;

        location / {
                proxy_set_header Host $host;
                proxy_set_header X-Real-IP $remote_addr;
                proxy_pass http://127.0.0.1:5501;
        }

        server_name your.hostname;
}
```

Technology stack
----------------

(for the curious)

Server: Debian Linux + Nginx + SQLite + .NET Core (with ASP.NET Core, Entity Framework Core)
Client: Xamarin.Forms