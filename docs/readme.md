# Разобрать каждое требование на составляющие
Ниже представлены итоги выписывания команд запросов при первом проходе по ТЗ.

## Таск трекер

### 1 требование
Таск-трекер должен быть отдельным дашбордом и доступен всем сотрудникам компании UberPopug Inc.

```
Actor: Account
Query: Dashboard data (???)
```

### 2 требование
Авторизация в таск-трекере должна выполняться через общий сервис авторизации UberPopug Inc (у нас там инновационная система авторизации на основе формы клюва).
```
Actor: Account
Command: Login to Task Tracker
Event: Account Loged in
```
### 3 требование
В таск-трекере должны быть только задачи. Проектов, скоупов и спринтов нет, потому что они не умещаются в голове попуга.
```
(уточняет содержимое системы)
```
### 4 требование
Новые таски может создавать кто угодно (администратор, начальник, разработчик, менеджер и любая другая роль). У задачи должны быть описание, статус (выполнена или нет) и попуг, на которого заассайнена задача.
```
Actor: Account
Command: Create task
Event: Task Created
Data: Task (description, status, assignee, createdBy)
```
### 5 требование
Менеджеры или администраторы должны иметь кнопку «заассайнить задачи», которая возьмёт все открытые задачи и рандомно заассайнит каждую на любого из сотрудников
```
Actor: Manager/Admin
Command: Bulk Assign tasks
Event: Task assignee changed
Data: Accounts(role + id), Tasks (status + id)
```
### 6 требование
Каждый сотрудник должен иметь возможность видеть в отдельном месте список заассайненных на него задач + отметить задачу выполненной.
```
Actor: Account
Query: List assigned tasks
Data: Task (description, status, assignee)
```
```
Actor: Account
Command: Mark task as completed
Event: Task completed
Data: Task (id + status)
```
## Аккаунтинг
### 1 требование
Аккаунтинг должен быть в отдельном дашборде и доступным только для администраторов и бухгалтеров. У обычных попугов доступ к аккаунтингу тоже должен быть. Но только к информации о собственных счетах (аудит лог + текущий баланс). У админов и бухгалтеров должен быть доступ к общей статистике по деньгами заработанным (количество заработанных топ-менеджментом за сегодня денег + статистика по дням).
```
Actor: Account
Query: Get Account status
Data: Audit log, Current balance
```
```
Actor: Admin/Accountant
Query: Get Top Management Statistic
Data: Today income + day-to-day report
```
### 2 требование
Авторизация в дешборде аккаунтинга должна выполняться через общий сервис аутентификации UberPopug Inc.
```
Actor: Account
Command: Login to Accounting
Event: Account Loged In
```
### 3 требование
У каждого из сотрудников должен быть свой счёт, который показывает, сколько за сегодня он получил денег. У счёта должен быть аудитлог того, за что были списаны или начислены деньги, с подробным описанием каждой из задач.
```
Actor: Account
Query: Get Account status
Data: Audit log, Current balance, task (description)
```
### 4 требование
Расценки
```
Actor: Event <Task created>
Command: Estimate cost
Event: Task estimated
Data: Task (id)
```
```
Actor: Event <Task assignee changed>, Event <Task completed>
Command: Append transaction log
Data: Task (description, id), amount
Event: Transaction Loged
```
```
Actor: Event <Transaction Loged>
Command: Update balance
Data: Amount
```
### 5 требование
Дешборд должен выводить количество заработанных топ-менеджментом за сегодня денег. 
```
Actor: Account
Query: Get top management balance
Data: Today income
```
### 6 требование
В конце дня необходимо:считать сколько денег сотрудник получил за рабочий день, отправлять на почту сумму выплаты.
```
Actor: Cron
Command: Close operational day
Event: Operation day closed
Data: date
```
```
Actor: Event<Close operational day>
Command: Prepare daily report
Event: Daily report ready * X (per account)
Data: Daily income per Account (account + daily income)
```
### 7 требование 
После выплаты баланса (в конце дня) он должен обнуляться, и в аудитлоге всех операций аккаунтинга должно быть отображено, что была выплачена сумма.
```
Actor: Event<Daily report ready>
Command: Perform payment
Event: Payment completed
Data: Amount, account id, date
```
```
Actor: Event<Payment completed>
Command: Send email report
Data: Email content with income
```
```
Actor: Event<Payment completed>
Command: Append transaction log
Data: Date, amount
```
### 8 требование
Дашборд должен выводить информацию по дням, а не за весь период сразу.

(уточняет предыдущие пункты)

## Аналитика
### 1 требование
Аналитика — это отдельный дашборд, доступный только админам.
```
Actor: Admin
Command: Login to analytics
Event: Account Loged in
```
### 2 требование
Нужно указывать, сколько заработал топ-менеджмент за сегодня и сколько попугов ушло в минус.
```
Actor: Admin
Query: Get daily report
Data: Daily income + debt popugs report
```
### 3 требование
Нужно показывать самую дорогую задачу за день, неделю или месяц.
```
Actor: Admin
Query: Get top priced tasks report
Data: Report - Tasks (description, id, cost) + period (date/month/year)
```

# Бизнес цепочки
* `Регистрация -> Создание аккаунтов в системах`
* `Авторизация -> Создание аккаунтов в системах`
* `Создание задачи -> Назначение стоимости задачи`
* `Ассайн задачи -> Добавление записи в лог транзакций (списание)`
* *  `-> Обновление баланса попуга`
* *  `-> Обновление статистики и аналитики`
* `Выполнение задачи -> Добавление записи в лог транзакций (пополнение)`
* *  `-> Обновление баланса попуга`
* *  `-> Обновление статистики и аналитики`
* `Закрытие операционного дня -> Расчет выплат -> Выплата -> Добавление записи в лог транзакций (выплата)`
* *  `-> Обновление баланса попуга`
* *  `-> Email попугу`
* *  `-> Обновление статистики и аналитики`