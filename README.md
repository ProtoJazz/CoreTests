# CoreTests
Test app for dotnetcore, with dokku

# Setting up a dotnetcore asp.net app for use with a Dokku/Linux Based docker container, deployed with circleci

There were a few snags setting this up. First is that by default asp.net wants to use MSSQL, but thats not really a thing on linux. So we need to use a linux compatable database. For this project I used postgres

Second snag, Dokku doesnt want to run the migrations in app.json. Thats a defect being worked on, have to just run them by hand for now

Third, By default nginx adds `proxy_set_header Connection "Upgrade";` to the config, but Kestrel wants `proxy_set_header Connection $http_connection;`  Without this posts with a body always return 400

I also had a bit of a compatability issue with the way Dokku exports the Env var for the postgres connection. Maybe theres a better way to solve that, but I just split on the colons and made it work

# Instructions

Assuming you have a brand new dotnet core project, you want to add the packages for postgresql
`<PackageReference Include="Npgsql.EntityFrameworkCore.PostgreSQL" Version="2.0.0" />` 
To your csproj. You also need to make sure you have all the packages you need in your `DotNetCliToolReference` list 
```

  <ItemGroup>
    <DotNetCliToolReference Include="Microsoft.VisualStudio.Web.CodeGeneration.Tools" Version="2.0.0" />
      <DotNetCliToolReference Include="Microsoft.EntityFrameworkCore.Tools" Version="2.0.0" />
      <DotNetCliToolReference Include="Microsoft.EntityFrameworkCore.Tools.DotNet" Version="2.0.0" />

</ItemGroup>
```
You will need to setup your database and entity framework, I followed the instructions here : https://docs.microsoft.com/en-us/aspnet/core/data/ef-mvc/intro but made some changes to work with the Npgsql package, you can see them here https://docs.microsoft.com/en-us/aspnet/core/data/ef-mvc/intro

` services.AddEntityFrameworkNpgsql().AddDbContext<BloggingContext>(options => options.UseNpgsql(connectionString));`

Once you have that setup, you should have a project that you can actually use locally (You may need to setup postgresql locally, if you dont have it) The next steps will be how to set it up to deploy

Next we will setup Dokku, since setting up circle ci first can lead to a ton of emails, and waiting. Its best to get your build running, then automate

# Dokku setup

For this project, I just used the premade digital ocean droplet: https://www.digitalocean.com/community/tutorials/how-to-use-the-digitalocean-dokku-application  Its pretty easy to use, but if you want to install on your own server, check here http://dokku.viewdocs.io/dokku/getting-started/installation/ 

Once you have Dokku up and running, run the following (For this example the names coretest, and coretestdb will be the name of our app, and our db, feel free to change them if you need to)

This will create the app
`dokku apps:create coretest` 

Then isntall the postgres plugin
`sudo dokku plugin:install https://github.com/dokku/dokku-postgres.git postgres`

Create the postgres db
`dokku postgres:create coretestdb`

Link it
`dokku postgres:link coretestdb coretest`

Now you should have a postgres db, exporting an environment var on your dokku app. Now we have to setup our project to be dokku ready, and push. 

# Project setup for Dokku

Add this dockerfile to the root of the project (Where the sln is)
https://github.com/ProtoJazz/CoreTests/blob/master/Dockerfile
This tells dokku/docker how to deploy your app.

Add this nginx.conf.sigil to where ever your csproj is
https://github.com/ProtoJazz/CoreTests/blob/master/Core3/nginx.conf.sigil
This tells dokku how to configure nginx

Add app.json to where the csproj is (Currently broken, so probably dont do this)
https://github.com/ProtoJazz/CoreTests/blob/751e54c44c5cf32c3f1c5a213f53f125d96e7589/Core3/app.json

Now push to dokku
```
git remote add dokku dokku@dokku.example.com:coretest
git push dokku master
```

If everything went well your app should deploy and we can setup circle ci

# Circle Ci setup
Setup a https://circleci.com/ account

Find the your project repo and click Build Project

Create a new SSH key
```
ssh dokku
cd ~/.ssh
ssh-keygen -t rsa    # save as circleci.id_rsa
sudo dokku ssh-keys:add circleci ./circleci.id_rsa.pub
cat ~/.ssh/circleci.id_rsa
```

Add your Dokku SSH key to Circle Ci under Project Settings -> SSH Permissions -> Add SSH Key

Make a folder at the root of your project, called .circleci

Inside the .circleci folder, add https://github.com/ProtoJazz/CoreTests/blob/master/.circleci/config.yml
This will checkout, echo hello world (Should be removed), dotnet restore, dotnet build, then deploy to dokku if this is the master branch. 
This step could also include tests, or anything else you want to run before deploy. 

If everything worked well, you should now have a dotnetcore CI/CD app
