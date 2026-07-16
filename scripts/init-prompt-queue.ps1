$prompts = @(
    @{ Id = 58;  Slug = "prepare-staging-deployment"; Title = "Prepare Staging Deployment" },
    @{ Id = 59;  Slug = "release-build-verification"; Title = "Release Build Verification" },
    @{ Id = 60;  Slug = "smoke-test-scripts"; Title = "Smoke Test Scripts" },
    @{ Id = 61;  Slug = "release-tag-prep"; Title = "Release Tag Prep" },

    @{ Id = 62;  Slug = "staging-verification"; Title = "Staging Verification" },
    @{ Id = 63;  Slug = "bug-bash"; Title = "Bug Bash" },
    @{ Id = 64;  Slug = "final-release-gate"; Title = "Final Release Gate" },
    @{ Id = 65;  Slug = "release-tag-instructions"; Title = "Release Tag Instructions" },

    @{ Id = 66;  Slug = "github-release-prep"; Title = "GitHub Release Prep" },
    @{ Id = 67;  Slug = "uat-plan"; Title = "UAT Plan" },
    @{ Id = 68;  Slug = "production-readiness"; Title = "Production Readiness" },
    @{ Id = 69;  Slug = "issue-triage"; Title = "Issue Triage" },

    @{ Id = 70;  Slug = "execute-uat-pilot"; Title = "Execute UAT Pilot" },
    @{ Id = 71;  Slug = "bug-fix-sprint"; Title = "Bug Fix Sprint" },
    @{ Id = 72;  Slug = "ux-consistency-pass"; Title = "UX Consistency Pass" },
    @{ Id = 73;  Slug = "v020-roadmap"; Title = "v0.2.0 Roadmap" },

    @{ Id = 74;  Slug = "content-review-workflow"; Title = "Content Review Workflow" },
    @{ Id = 75;  Slug = "review-content-workflow"; Title = "Review Content Workflow" },
    @{ Id = 76;  Slug = "docs-content-workflow"; Title = "Docs Content Workflow" },
    @{ Id = 77;  Slug = "commit-summary"; Title = "Commit Summary" },

    @{ Id = 78;  Slug = "better-publishing"; Title = "Better Publishing" },
    @{ Id = 79;  Slug = "review-better-publishing"; Title = "Review Better Publishing" },
    @{ Id = 80;  Slug = "docs-better-publishing"; Title = "Docs Better Publishing" },
    @{ Id = 81;  Slug = "commit-summary"; Title = "Commit Summary" },

    @{ Id = 82;  Slug = "student-facing-learning-pages"; Title = "Student-facing Learning Pages" },
    @{ Id = 83;  Slug = "review-student-learning-pages"; Title = "Review Student Learning Pages" },
    @{ Id = 84;  Slug = "docs-student-learning-pages"; Title = "Docs Student Learning Pages" },
    @{ Id = 85;  Slug = "commit-summary"; Title = "Commit Summary" },

    @{ Id = 86;  Slug = "student-progress-learning-tracking"; Title = "Student Progress Learning Tracking" },
    @{ Id = 87;  Slug = "review-student-progress"; Title = "Review Student Progress" },
    @{ Id = 88;  Slug = "docs-student-progress"; Title = "Docs Student Progress" },
    @{ Id = 89;  Slug = "commit-summary"; Title = "Commit Summary" },

    @{ Id = 90;  Slug = "basic-reporting-admin-dashboard"; Title = "Basic Reporting Admin Dashboard" },
    @{ Id = 91;  Slug = "review-reporting-analytics"; Title = "Review Reporting Analytics" },
    @{ Id = 92;  Slug = "docs-reporting"; Title = "Docs Reporting" },
    @{ Id = 93;  Slug = "commit-summary"; Title = "Commit Summary" },

    @{ Id = 94;  Slug = "notification-email-foundation"; Title = "Notification Email Foundation" },
    @{ Id = 95;  Slug = "review-notification-email"; Title = "Review Notification Email" },
    @{ Id = 96;  Slug = "docs-notification-email"; Title = "Docs Notification Email" },
    @{ Id = 97;  Slug = "commit-summary"; Title = "Commit Summary" },

    @{ Id = 98;  Slug = "production-deployment-hardening"; Title = "Production Deployment Hardening" },
    @{ Id = 99;  Slug = "backup-restore-disaster-recovery"; Title = "Backup Restore Disaster Recovery" },
    @{ Id = 100; Slug = "production-monitoring-operations-checklist"; Title = "Production Monitoring Operations Checklist" },
    @{ Id = 101; Slug = "production-hardening-commit-summary"; Title = "Production Hardening Commit Summary" },

    @{ Id = 102; Slug = "content-quality-qa-system"; Title = "Content Quality QA System" },
    @{ Id = 103; Slug = "review-content-quality"; Title = "Review Content Quality" },
    @{ Id = 104; Slug = "docs-content-quality"; Title = "Docs Content Quality" },
    @{ Id = 105; Slug = "clean-commit-summary-content-quality-qa-system"; Title = "Clean Commit Summary Content Quality QA System" },

    @{ Id = 106; Slug = "content-versioning-revision-history"; Title = "Content Versioning Revision History" },
    @{ Id = 107; Slug = "review-content-versioning"; Title = "Review Content Versioning" },
    @{ Id = 108; Slug = "docs-content-versioning"; Title = "Docs Content Versioning" },
    @{ Id = 109; Slug = "commit-summary"; Title = "Commit Summary" },

    @{ Id = 110; Slug = "content-bulk-operations"; Title = "Content Bulk Operations" },
    @{ Id = 111; Slug = "review-harden-content-bulk-operations"; Title = "Review Harden Content Bulk Operations" },
    @{ Id = 112; Slug = "docs-content-bulk-operations"; Title = "Docs Content Bulk Operations" },
    @{ Id = 113; Slug = "commit-summary"; Title = "Commit Summary" },

    @{ Id = 114; Slug = "advanced-import-validation"; Title = "Advanced Import Validation" },
    @{ Id = 115; Slug = "review-advanced-import"; Title = "Review Advanced Import" },
    @{ Id = 116; Slug = "documentation-advanced-import-validation-content-migration-tools"; Title = "Documentation Advanced Import Validation Content Migration Tools" },
    @{ Id = 117; Slug = "commit-summary"; Title = "Commit Summary" },

    @{ Id = 118; Slug = "better-public-search-discovery"; Title = "Better Public Search Discovery" },
    @{ Id = 119; Slug = "review-public-search"; Title = "Review Public Search" },
    @{ Id = 120; Slug = "docs-public-search"; Title = "Docs Public Search" },
    @{ Id = 121; Slug = "commit-summary"; Title = "Commit Summary" },

    @{ Id = 122; Slug = "learning-recommendations"; Title = "Learning Recommendations" },
    @{ Id = 123; Slug = "review-learning-recommendations"; Title = "Review Learning Recommendations" },
    @{ Id = 124; Slug = "update-documentation-learning-recommendations-continue-learning"; Title = "Update Documentation Learning Recommendations Continue Learning" },
    @{ Id = 125; Slug = "commit-summary"; Title = "Commit Summary" },

    @{ Id = 126; Slug = "practice-system-spaced-repetition"; Title = "Practice System Spaced Repetition" },
    @{ Id = 127; Slug = "review-practice-system"; Title = "Review Practice System" },
    @{ Id = 128; Slug = "update-documentation-practice-system-spaced-repetition-foundation"; Title = "Update Documentation Practice System Spaced Repetition Foundation" },
    @{ Id = 129; Slug = "commit-summary"; Title = "Commit Summary" },

    @{ Id = 130; Slug = "learning-goals-daily-study-plan"; Title = "Learning Goals Daily Study Plan" },
    @{ Id = 131; Slug = "review-learning-goals-study-plan"; Title = "Review Learning Goals Study Plan" },
    @{ Id = 132; Slug = "update-documentation-learning-goals-daily-study-plan"; Title = "Update Documentation Learning Goals Daily Study Plan" },
    @{ Id = 133; Slug = "commit-summary"; Title = "Commit Summary" },

    @{ Id = 134; Slug = "streaks-achievements-motivation"; Title = "Streaks Achievements Motivation" },
    @{ Id = 135; Slug = "review-streaks-achievements"; Title = "Review Streaks Achievements" },
    @{ Id = 136; Slug = "docs-motivation"; Title = "Docs Motivation" },
    @{ Id = 137; Slug = "commit-summary"; Title = "Commit Summary" },

    @{ Id = 138; Slug = "student-weekly-learning-report"; Title = "Student Weekly Learning Report" },
    @{ Id = 139; Slug = "eview-harden-student-weekly-summary-learning-report"; Title = "Review Harden Student Weekly Summary Learning Report" },
    @{ Id = 140; Slug = "docs-weekly-learning-report"; Title = "Docs Weekly Learning Report" },
    @{ Id = 141; Slug = "clean-commit-summary"; Title = "Clean Commit Summary" },

    @{ Id = 142; Slug = "v020-release-candidate-stabilization"; Title = "v0.2.0 Release Candidate Stabilization" },
    @{ Id = 143; Slug = "perform-security-permission-audit"; Title = "Perform Security Permission Audit" },
    @{ Id = 144; Slug = "v020-uat-smoke-release-docs"; Title = "v0.2.0 UAT Smoke Release Docs" },
    @{ Id = 145; Slug = "v020-rc-commit-tag-prep"; Title = "v0.2.0 RC Commit Tag Prep" },

    @{ Id = 146; Slug = "smoke-test"; Title = "Smoke Test" },
    @{ Id = 147; Slug = "v020-uat-tracker"; Title = "v0.2.0 UAT Tracker" },
    @{ Id = 148; Slug = "prompt"; Title = "v0.2.0 Bug Fix Sprint" },
    @{ Id = 149; Slug = "v020-rc-final-decision"; Title = "v0.2.0 RC Final Decision" },

    @{ Id = 150; Slug = "prompt"; Title = "Staging Deployment RC Tag Prep" },
    @{ Id = 151; Slug = "execute-staging-smoke-uat"; Title = "Execute Staging Smoke UAT" },
    @{ Id = 152; Slug = "final-v020-release-decision"; Title = "Final v0.2.0 Release Decision" },
    @{ Id = 153; Slug = "prompt"; Title = "Final Release Notes Monitoring" },

    @{ Id = 154; Slug = "production-go-live-execution-prep"; Title = "Production Go-Live Execution Prep" },
    @{ Id = 155; Slug = "production-smoke-test"; Title = "Production Smoke Test" },
    @{ Id = 156; Slug = "prompt"; Title = "Post-release Monitoring Hotfix Triage" },
    @{ Id = 157; Slug = "release-closure-v030-planning-gate"; Title = "Release Closure v0.3.0 Planning Gate" },

    @{ Id = 158; Slug = "v030-roadmap-prioritization"; Title = "v0.3.0 Roadmap Prioritization" },
    @{ Id = 159; Slug = "v030-scope-decision-mvp-slice"; Title = "v0.3.0 Scope Decision MVP Slice" },
    @{ Id = 160; Slug = "v030-architecture-risk-review"; Title = "v0.3.0 Architecture Risk Review" },
    @{ Id = 161; Slug = "v030-implementation-plan-prompt-queue"; Title = "v0.3.0 Implementation Plan Prompt Queue" }
)

New-Item -ItemType Directory -Force prompts\queue | Out-Null
New-Item -ItemType Directory -Force logs\codex-queue | Out-Null

$items = @()

foreach ($p in $prompts) {
    $idText = "{0:D3}" -f $p.Id
    $file = "prompts/queue/$idText-$($p.Slug).md"
    $windowsFile = $file -replace "/", "\"

    if (-not (Test-Path $windowsFile)) {
        $content = @"
---
id: $idText
title: $($p.Title)
status: pending
phase: queue
---

PASTE FULL PROMPT CONTENT HERE
"@
        Set-Content -Path $windowsFile -Value $content -Encoding UTF8
    }

    $dependsOn = @()
    if ($p.Id -gt 58) {
        $dependsOn = @($p.Id - 1)
    }

    $items += [ordered]@{
        id = $p.Id
        title = $p.Title
        file = $file
        status = "pending"
        phase = "queue"
        attempts = 0
        max_attempts = 3
        depends_on = $dependsOn
        run_build_after = $true
        run_test_after = $true
        log_file = "logs/codex-queue/$idText-$($p.Slug).log"
        notes = ""
    }
}

$queue = [ordered]@{
    project = "EnglishMaster"
    queue_version = "1.0.0"
    current_range = [ordered]@{
        start = 58
        end = 161
    }
    items = $items
}

$queue | ConvertTo-Json -Depth 10 | Set-Content -Path "prompt-queue.json" -Encoding UTF8

Write-Host "Prompt queue initialized."
Write-Host "Prompt files created:" $prompts.Count
Write-Host "Next step: paste full prompt content into each .md file before running queue."
