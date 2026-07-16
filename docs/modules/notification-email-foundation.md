# Notification And Email Foundation

## Overview

The notification and email foundation adds durable records for in-app notifications and queued email messages. It is designed as a small platform service that other modules can call when content review, publishing, user management, or quiz workflows need to notify users.

## Domain

- `Notification` stores recipient, notification type, event type, title, message, status, timestamps, and error details.
- `EmailMessage` stores recipient email, subject, body, HTML flag, delivery status, timestamps, and error details.

Notification statuses are `Pending`, `Sent`, `Failed`, `Read`, and `Cancelled`. Email statuses are `Pending`, `Sent`, and `Failed`.

## Application Services

- `INotificationService` creates notifications for future workflow integrations.
- `IEmailSender` defines the email delivery adapter boundary.
- CQRS handlers support creating notifications, marking notifications read/sent/failed, searching notifications, queueing email messages, marking email sent/failed, and searching email messages.

## Infrastructure

EF Core persists notifications and email messages with indexes for status, event type, recipient, recipient email, and created date. The initial development email sender logs messages instead of sending through an external provider.

## UI

- `/learn/notifications` shows the authenticated user's notifications and unread count.
- `/admin/notifications` searches notification records for administrators.
- `/admin/email-messages` searches queued email messages for administrators.

## Follow-Up Integration

Content review, publishing, security, and quiz workflows can call `INotificationService` and queue email through the email application service when their specific event timing is finalized.
