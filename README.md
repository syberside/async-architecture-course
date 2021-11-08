# async-architecture-course
Домашние задания по курсу Асинхронная архитектура Федора Борщева (https://education.borshev.com/architecture)

# Технологии
* .NET Core 3.1
* ASP.NET MVC
* EntityFramework Core 5
* docker-compose
* Postgres
* Kafka
* IdentityServer4

# Как запустить
* установить Docker
* перейти в папку `src\aTES`
* запустить инфраструктурные зависимости с помощью `docker-compose up -d`
* собрать и запустить проекты
* перейти в GUI Identity сервиса (`https://localhost:5001`)
* выбрать пункт меню `Users`, авторизоваться под учетной записью `root` с паролем `pwd`
* создать несколько пользователей с разными ролями, выбрать в меню Logout
* перейти в таск-трекер или любой другой сервис