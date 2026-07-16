# Student Dashboard

## Route

```text
/learn/dashboard
```

## Purpose

The student dashboard gives authenticated learners a quick view of continue learning items and grouped recommendations.

## Sections

- Continue Learning
- Recommended Courses
- Recommended Lessons
- Recommended Words
- Recommended Grammar
- Recommended Quizzes
- Review Weak Areas
- Practice Due
- Start Practice
- Today's Study Plan
- Active Goals
- Study Summary
- Generate Today Plan
- Current Streak
- Achievement Summary
- Recent Activity
- Weekly Report

## States

The page includes loading, error, and empty states. It links to `/learn/recommendations` for the full recommendation page, `/learn/practice` for spaced repetition practice, `/learn/goals` for learning goals, and `/learn/study-plan` for the daily plan.

## Practice Card

The practice card is an authenticated learner shortcut into the practice system. It should show due practice context when available and provide a clear start action.

Practice data comes from `/api/v1/me/practice/summary`.

## Study Plan And Goals

The dashboard includes a compact view of today's study plan, active goals, plans this week, and completed study minutes.

Study plan and goal data comes from:

```text
/api/v1/me/study-plan/today
/api/v1/me/study-plan/summary
/api/v1/me/learning-goals/summary
```

## Motivation

The dashboard includes current streak and earned achievement count. Motivation data comes from:

```text
/api/v1/me/motivation/summary
```

## Weekly Report Card

The dashboard includes a compact weekly report card. When a current-week report exists, it shows study minutes, completed lessons, practice sessions, and quiz average with a link to `/learn/reports/current-week`.

When no current-week report exists, the card shows an empty state and a generate action.

Weekly report data comes from:

```text
/api/v1/me/learning-reports/current-week
POST /api/v1/me/learning-reports/current-week/generate
```
