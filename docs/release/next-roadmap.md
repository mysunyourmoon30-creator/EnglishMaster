# Next Roadmap

## Operating Rule

The 24 AI learning modules are a selectable engineering knowledge set, not 24
mandatory EnglishMaster product modules.

- Select a module only when the work item matches its trigger.
- Record `Required`, `Conditional`, or `Not Applicable` during planning.
- `Not Applicable` is the default when the product does not use an LLM, RAG,
  vector database, tool calling, or agent orchestration.
- A single reviewer may cover more than one role, but every triggered review
  lens must produce evidence.
- This roadmap does not approve AI Tutor, RAG, or multi-agent product scope.
  Those remain outside v0.4.0 until the release owner explicitly changes the
  scope lock.

## Recommended Next Step

Native no-Docker local browser UAT is complete. On 2026-08-04, the release owner
accepted local UAT plus disposable CI staging as the MVP release gate; see
`docs/release/mvp-release-gate-decision-2026-08-04.md`. Persistent staging is a
deferred operations follow-up and must not be reported as complete.

The next ordered product-planning step is to define the V1-F06 task contract.
The roadmap identifies Student Progress as the next business module, while the
current task queue ends at V1-F05. Do not begin implementation until V1-F06 has
an explicit scope, dependencies, allowed paths, and verification checks.

The current release path uses Modules 22-24 directly. Modules 5 and 8 are
architecture guardrails for API contracts and structured import/export data;
they do not create an AI feature by themselves.

## Selective AI Module Matrix

| Module | Topic | Roadmap use | Activation trigger | Primary review role |
| --- | --- | --- | --- | --- |
| 1 | Evolution of AI / วิวัฒนาการของปัญญาประดิษฐ์ (AI) | Reference only | Onboarding or course content needs historical context | AI Engineer |
| 2 | LLM Architecture / สถาปัตยกรรมโมเดลภาษาขนาดใหญ่ (LLM) | Reference only | Model selection, self-hosting, or architecture trade-off | AI Engineer |
| 3 | The Inference Lifecycle / วงจรการอนุมานของโมเดล | Conditional AI | An LLM request path needs latency, streaming, caching, or lifecycle analysis | AI Engineer |
| 4 | Deterministic vs. Stochastic / แบบกำหนดผลแน่นอนเทียบกับแบบสุ่ม | Conditional AI | Reproducibility, temperature, sampling, tests, or evaluation criteria matter | AI Engineer |
| 5 | API-First AI / การพัฒนาระบบ AI โดยยึด API เป็นหลัก | Architecture guardrail now | Any AI integration or new external API boundary is proposed | Developer; add AI Engineer only for AI integration |
| 6 | Prompt Engineering for Developers / การออกแบบพรอมต์สำหรับนักพัฒนา | Conditional AI | A production prompt or prompt template is added or changed | AI Engineer + English Teacher |
| 7 | Advanced Reasoning / การให้เหตุผลขั้นสูง | Conditional advanced | A measured use case cannot be solved reliably with one bounded prompt | AI Engineer |
| 8 | Structured Outputs / ผลลัพธ์แบบมีโครงสร้าง | Architecture guardrail now | DTO/schema output, structured import/export, or model-generated data crosses a boundary | Developer; add AI Engineer only for model output |
| 9 | Function & Tool Calling / การเรียกใช้ฟังก์ชันและเครื่องมือ | Conditional AI | A model is allowed to call application tools or APIs | AI Engineer + Security/System |
| 10 | Managing LLM Failures / การจัดการความล้มเหลวของ LLM | Required with any LLM | Any production LLM dependency is introduced | AI Engineer + Developer |
| 11 | Token Optimization / การใช้โทเคนอย่างมีประสิทธิภาพ | Conditional AI | Production measurements show cost, context, or latency pressure | AI Engineer |
| 12 | Designing Complex Reasoning/Prompt Chains / การออกแบบสายงานการให้เหตุผลและพรอมต์ที่ซับซ้อน | Conditional advanced | A workflow requires more than one model step and has evaluation evidence | AI Engineer + Developer |
| 13 | RAG Architecture / สถาปัตยกรรม RAG | Conditional RAG | Approved feature requires grounded retrieval from owned content | AI Engineer + Security/System |
| 14 | Data Ingestion / การนำเข้าข้อมูล | Conditional RAG | Approved RAG corpus needs ingestion, validation, and lifecycle ownership | AI Engineer + Developer |
| 15 | Chunking Strategies / กลยุทธ์การแบ่งข้อมูลเป็นส่วนย่อย | Conditional RAG | Retrieval quality depends on document segmentation | AI Engineer + English Teacher |
| 16 | Embedding Models / โมเดล Embedding | Conditional RAG | Embedding model selection is required | AI Engineer |
| 17 | Vector Database Internals / กลไกภายในของฐานข้อมูลเวกเตอร์ | Conditional RAG | A vector store is approved after scale and operational analysis | AI Engineer + Security/System |
| 18 | Similarity Search / การค้นหาด้วยความคล้ายคลึง | Conditional RAG | Retrieval ranking, filtering, or relevance tuning is implemented | AI Engineer |
| 19 | RAG Evaluation / การประเมินระบบ RAG | Required with RAG | Any RAG feature reaches acceptance testing | AI Engineer + English Teacher |
| 20 | Multi-Agent Systems / ระบบหลายเอเจนต์ | Conditional agent | A single deterministic workflow is proven insufficient | AI Engineer + Security/System |
| 21 | Intelligent Routing / การจัดเส้นทางอัจฉริยะ | Conditional agent | Multiple models, tools, or workflows require measured routing logic | AI Engineer + Developer |
| 22 | API-Driven System Automation / การทำระบบอัตโนมัติผ่าน API | Required now | Deployment, import/export, monitoring, or operational automation | Developer + Security/System |
| 23 | Secure Production Deployment / การนำระบบขึ้น Production อย่างปลอดภัย | Required now | Every staging or production release | Security/System + Developer |
| 24 | Reliability Engineering / วิศวกรรมความน่าเชื่อถือของระบบ | Required now | Every production-bound workflow or external dependency | Developer + Security/System |

## Delivery Phases

### Phase A: Current Release And Staging

Use Modules 22, 23, and 24. Apply Modules 5 and 8 only as design guardrails.

Status updated 2026-08-04:

Native local UAT passed public grammar, dashboard plus 28 admin list routes,
Category create/detail/edit, media rejection/acceptance, import row errors, and
logout. The release owner accepted the combined local and disposable CI
evidence as the MVP release gate on 2026-08-04. Persistent staging remains a
deferred operations follow-up until a target environment and secure deployment
access are configured.

1. Complete in code — API and Web expose `/health`, `/health/live`, and
   `/health/ready`; API readiness checks database connectivity.
2. Complete in code — structured console/rolling-file logging and
   `SystemHealthWorker` alert thresholds exist. Deployed metrics/alert
   destination verification remains an environment gate.
3. Complete — Development may migrate on startup; Staging and Production set
   `Database__ApplyMigrationsOnStartup=false` and use the reviewed release
   migration bundle before API startup.
4. Complete in disposable CI staging — Release Build run
   [30828858339](https://github.com/mysunyourmoon30-creator/EnglishMaster/actions/runs/30828858339)
   built Linux containers, installed SQL Server Full-Text, applied the reviewed
   migration bundle to a fresh database, waited for readiness, and passed the
   release smoke gate for revision `2784568`.
5. Complete in automation — `scripts/Invoke-EnglishMasterReleaseSmoke.ps1`
   passed login, admin dashboard API, anonymous public grammar content,
   protected API rejection, health, redirect, and logout checks. Persistent
   staging deployment and live browser workflows remain blocked until a target
   environment and secure deployment access are configured.

### Phase A Review Record

```text
Applicable AI modules:
- Required: 22 API-Driven System Automation, 23 Secure Production Deployment,
  24 Reliability Engineering
- Conditional: 5 API-First and 8 Structured Outputs as contract guardrails
- Not applicable: 1-4, 6-7, 9-21 because no LLM, RAG, vector database,
  model tool, or agent runtime was introduced

Review roles:
- English Teacher: Not applicable — no learner-facing language content changed
- UX/UI Designer: Not applicable — no Razor, CSS, navigation, or visual state changed
- AI Engineer: Not applicable — no model inference path exists
- Developer: Required — release workflow, migration switch, bundle, smoke tool,
  build, tests, and configuration validation reviewed
- Security/System: Required — explicit migrations, secret handling, HTTPS
  credential rule, health/readiness, 401 behavior, and deployment order reviewed
```

### Phase B: First Approved AI Feature

Do not start this phase until an AI feature is added to the product scope.
Start with Modules 3, 4, 5, 6, 8, 10, and 11. Add Module 9 only when tool
calling is required. Every feature needs a bounded API contract, timeout,
fallback behavior, cost limit, language-quality evaluation, and security
review.

### Phase C: RAG

Do not start with a vector database. First approve a grounded retrieval use
case and evaluation dataset. Then use Modules 13-19 as one complete slice;
Module 19 is a release gate, not optional follow-up work.

### Phase D: Multi-Agent Orchestration

Use Modules 7, 9, 12, 20, and 21 only after a simpler single-model or
deterministic workflow has failed a measured requirement. Require tool
allowlists, per-step budgets, bounded retries, audit logs, and a human approval
boundary for material side effects.

## Review Roles And Triggers

### English Teacher / ผู้ตรวจภาษาอังกฤษ

Required when work changes learner-facing English or Thai content, grammar
rules, examples, prompts that generate language content, quizzes, CEFR labels,
or translations.

Evidence:

- Grammar, naturalness, terminology, and Thai-English meaning are correct.
- Examples are pedagogically useful and match the intended CEFR level.
- Correct and incorrect examples cannot be confused.
- AI/RAG evaluation includes language-quality cases when applicable.

### UX/UI Designer / ผู้ตรวจ UX/UI

Required when work changes `.razor`, CSS, layout, navigation, forms, loading,
empty, error, or responsive states.

Evidence:

- The main action is obvious and reachable with keyboard and touch.
- Typography, font size, spacing, color, and contrast match the project.
- Content is readable on desktop and mobile without clipping or overflow.
- Loading, empty, validation, success, and failure states are understandable.
- Accessibility names, focus behavior, reduced motion, and status
  announcements are covered where relevant.

### AI Engineer / ผู้ตรวจระบบ AI

Required only when a work item activates an AI-specific use of Modules 1-21.
For Modules 5 and 8 in ordinary API, DTO, or import/export work with no model
inference, the Developer role owns the guardrail and AI Engineer is not
applicable.

Evidence:

- Model, prompt, sampling, context, schema, and evaluation choices are recorded.
- Failure, timeout, retry, fallback, token, latency, and cost limits are
  explicit.
- RAG work has retrieval and language-quality evaluation.
- Tool or agent work has bounded permissions and deterministic escape paths.

### Developer / ผู้ตรวจการพัฒนา

Required for every production code, API, schema, migration, configuration, or
automation change.

Evidence:

- Clean Architecture dependency direction and module ownership remain clear.
- Public contracts are narrow and versionable.
- Validation, cancellation, error states, observability, and idempotency are
  appropriate.
- Focused tests pass, followed by the relevant broader build/test suite.

### Security/System / ผู้ตรวจความปลอดภัยและระบบ

Required when work changes trust boundaries, anonymous/authenticated behavior,
roles or permissions, data exposure, uploads/imports, external APIs, model
tools, secrets, deployment, logging, background jobs, or reliability controls.

Evidence:

- Authentication, authorization, ownership, and least privilege are verified.
- Inputs, outputs, rate limits, sensitive data, logs, and error details are
  reviewed.
- Secrets and Data Protection keys are stored outside source control.
- Deployment includes rollback, health checks, monitoring, and incident
  signals.
- AI tools and agents cannot perform material side effects without the intended
  authorization or human approval boundary.

## Review Gate Per Work Item

Every work item must include this block:

```text
Applicable AI modules:
- Required:
- Conditional:
- Not applicable:

Review roles:
- English Teacher: Required / Not applicable — reason
- UX/UI Designer: Required / Not applicable — reason
- AI Engineer: Required / Not applicable — reason
- Developer: Required — evidence
- Security/System: Required / Not applicable — reason
```

A work item is not complete until all `Required` roles record findings or
explicitly state that no issues were found.

### Example: Public Grammar Pages

```text
Applicable AI modules:
- Required: 5 API-First, 8 Structured Outputs
- Conditional: 24 Reliability at release-level load and monitoring review
- Not applicable: 1-4, 6-7, 9-23 because the feature has no LLM, RAG,
  vector database, model tool, agent workflow, automation, or deployment
  change

Review roles:
- English Teacher: Required — grammar, examples, translations, and CEFR
- UX/UI Designer: Required — public topic/rule pages and responsive states
- AI Engineer: Not applicable — no AI inference path
- Developer: Required — API, DTO, persistence projection, UI client, tests
- Security/System: Required — anonymous boundary and active-content exposure
```

## Security Follow-Up

1. Add audit logging for user, role, permission, publishing, and import actions.
2. Add account lockout and password reset flows.
3. Add MFA for privileged users.
4. Add permission-aware navigation and UI command visibility.
5. Add explicit `401` and `403` integration coverage for high-risk endpoints.

## Content Operations Follow-Up

1. Add Category and Tag import.
2. Add structured import for Lessons, Courses, Books, and Quizzes.
3. Add import preview and dry-run mode.
4. Add full Quiz question and choice export where appropriate.

## Performance And Operations Follow-Up

1. Add paginated Category and Tag APIs before large catalogs.
2. Add typeahead lookup endpoints for large dropdowns.
3. Move publishing and large import/export work to background processing.
4. Add real PDF and DOCX rendering after choosing libraries deliberately.

## Product Roadmap

The next business module should be Student Progress. The MVP release gate was
accepted on 2026-08-04, but V1-F06 has not yet been defined in the task queue.
Create and approve its task contract before implementation. AI Tutor, RAG, and
multi-agent features still require separate scope approval.
