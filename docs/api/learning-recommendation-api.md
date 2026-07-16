# Learning Recommendation API

## Endpoints

All endpoints require authentication and use the current user from claims.

| Method | Route | Purpose |
| --- | --- | --- |
| `GET` | `/api/v1/me/learning/continue` | Continue learning items. |
| `GET` | `/api/v1/me/learning/recommendations` | Full recommendation summary. |
| `GET` | `/api/v1/me/learning/recommended-courses` | Recommended courses. |
| `GET` | `/api/v1/me/learning/recommended-lessons` | Recommended lessons. |
| `GET` | `/api/v1/me/learning/recommended-words` | Recommended words. |
| `GET` | `/api/v1/me/learning/recommended-grammar` | Recommended grammar. |
| `GET` | `/api/v1/me/learning/recommended-quizzes` | Recommended quizzes. |
| `GET` | `/api/v1/me/learning/review` | Review weak quiz areas. |
| `GET` | `/api/v1/me/learning/courses/{courseId}/next-lesson` | Next incomplete lesson for a course. |

## Query Parameters

- `limit`: optional, default 5, maximum 20.
- `cefrLevel`: optional recommendation CEFR override.

## Safety

The API never accepts a user id parameter. It only uses the authenticated user id claim. Quiz recommendations do not include answer data.

