![Popug aTES architecture diagram](/docs/Async%20architecture%20-%20lesson%200.drawio.png)
# Описание сервисов

Сервисы далее рассматриваются как black-box.
Подразумевается, что у каждого сервиса собственная БД и инфраструктура.

## UberPopug inc infra
Сервисы UberPopug за пределами скоупа проекта.

Предоставляют сервисы (вне скоупа проекта):
* SSO форму авторизации с поддержкой ПКИ (переносного клювного измерителя).
* Хранилище учетных записей попугов и сервисы по их обновлению.
* Хранилище способов оплаты и сервисы управления ими.
* Другие очень важные корпоративные сервисы.

## aTES UI
Веб интерфейс приложения.

Содержит отдельную страницу для каждого "дашборда".

Для авторизации перенаправляет на SSO форму UberPopug inc.

## aTES API
Backend приложение для веб интефрейса aTES.

Синхронно проверяет права пользователя (на текущем этапе считаем что сервисам самим не нужно этого делать) вызывая Profile & authorization service.
Для выполнения запрошенных пользователем операций синхронно вызывает другие сервисы.
Синхронно запрашивает нужные данные у сервисов и формирует ответ для UI.

Выделен в отдельный сервис, чтобы клиенту не нужно было делать множество запросов на различные сервисы (backend for frontend).

## Task Service
Сервис управления задачами.
Реализует как CRUD для задач, так и функционал для массового назначения попугов на задачу.

Асинхронно передает в Accounting Service событие завершения задачи. Содержимое события:
* ИД задачи
* ИД попуга-исполнителя
* Заголовок задачи
* Описание задачи

Почему выделен в отдельный сервис: инкапсуляция логики управления задачами.

## Accounting service
Сервис аккаунтинга.
Управляет балансом аккаунтов, ведет лог начислений и выплат. Также реализует фоновую задачу закрытия операционного дня по расписанию.

Хранит следующие данные:
* Стоимости задач
* Данные об аккаунте (баланс, лог, лог выплат)
* В логе выплат также хранит снапшет заголовка и описания задачи (для аудита)

Интеграции:
* двухсторонняя асинхронная интеграция с Payments service.
  *  отправляет ИД выплаты, сумму и идентификатор попуга
  *  принимает обновление статуса выплаты
* асинхронно отправляет в сервис нотификаций запрос на отправку уведомления попугу о выплате (отправляет ид попуга и сообщение).

Зачем выделен в отдельный сервис: инкапсуляция логики аккаунтинга и формирования отчетов (можно дробить дальше при возникновении необходимости в ходе развития проекта).

## Payments service
Сервис интеграции с платежной системой.

Хранит информацию, необходимую для интерации с платежной системой и логи взаимодействия с ней (попытки отправки и пр).

Интеграции:
* синхронно запрашивает от UberPopug inc информацию о счете попуга
* синхронно/асинхронно взаимодействует с сервисом выплат (зависит от сервиса)
* двухсторонняя асинхронная интеграция с Accounting service
*   отправляет идентификатор выплаты и обновленный статус
*   принимает запрос на выплату

Зачем выделен в отдельный сервис: инкапсуляция изменения связанных с выводом денег в одном сервисе (попуги могут захотеть использовать различные способы оплаты) + изоляция от особенностей внешних сервисов (в первом варианте системы выдачу средств может осуществлять кассирша Маша наличкой).

## Notifications service
Сервис отправки уведомлений попугам. Формирует письма и отправляет их.

Хранит данные о письме и логи интеграции с SMTP (попытки отправки и пр).

Интеграции:
* синхронно отправляет по SMTP письма
* асинхронно принимает запросы на отправку письма (ид попуга + выплаченная сумма).
* синхронно запрашивает у Profile and authorization service email и имя попуга.

Почему выделен в отдельный сервис: изоляция изменений связанных с доставкой уведомлений (попуги могут захотеть получать чеки голубиной почтой) + изоляция от внешних сервисов.

## Profile and authorization service
Хранит:
* информацию о токенах авторизации
* имя попуга и email
* роли попуга

Интегируется с сервисами UberPopug inc для авторизации пользователей
* синхронно запрашивает данные, если их нет в собственной БД
* асинхронно получает обновления имени и email попуга

Почему выделен в отдельный сервис: при усложении проекта может возникнуть необходимость проверять права из более чем одного сервиса + изоляция от сервиса авторизации по клюву.

# Что делать если упадет сеть, БД и пр.
Система спроектирована (в основном) как монолит в целях обучения. Некоторые нюансы:
* в случае недоступности сервиса авторизации вся система не доступна (отображаем экран "система не доступна")
* в случае недоступности UI API отображаем экран "система не доступна" (через веб-прокси)
* в случае недоступности сервиса аккаунтинга задачи можно завершать (сообщения складываются в очередь). В случае переполнения - выплаты попугов теряются.
* поведение в случае недоступности любого другого сервиса аналогичное
* вопросы обслуживания БД и очередей отданы попугам-админам
* от сервисов UberPopug inc изоляции нет, вызываем напрямую

# Что хотелось бы изменить в ходе курса
* максимально изолировать сервисы друг от друга, чтобы падения одного не влияли на другой
* решить вопросы связанные с потерей данных при недоступности сервисов
* добавить изоляцию от сервисов UberPopug inc
* описать и поддержать возможную эволюцию системы 