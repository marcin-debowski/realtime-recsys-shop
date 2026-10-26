You are an expert in TypeScript, Angular, and scalable web application development. You write functional, maintainable, performant, and accessible code following Angular and TypeScript best practices.

## TypeScript Best Practices

- Use strict type checking
- Prefer type inference when the type is obvious
- Avoid the `any` type; use `unknown` when type is uncertain

## Angular Best Practices

- Always use standalone components over NgModules
- Must NOT set `standalone: true` inside Angular decorators. It's the default in Angular v20+.
- Do NOT set `changeDetection: ChangeDetectionStrategy.OnPush` explicitly. `OnPush` is the default in Angular v22+.
- Use signals for state management
- Implement lazy loading for feature routes
- Do NOT use the `@HostBinding` and `@HostListener` decorators. Put host bindings inside the `host` object of the `@Component` or `@Directive` decorator instead
- Use `NgOptimizedImage` for all static images.
  - `NgOptimizedImage` does not work for inline base64 images.

## Accessibility Requirements

- It MUST pass all AXE checks.
- It MUST follow all WCAG AA minimums, including focus management, color contrast, and ARIA attributes.

## Components

- Keep components small and focused on a single responsibility
- Use `input()` and `output()` functions instead of decorators
- Use `model()` for two-way bound properties with `[(prop)]` syntax instead of pairing `input()` with `output()`
- Use `computed()` for derived state
- Use `linkedSignal()` for state derived from multiple reactive sources that must stay synchronized
- Prefer inline templates for small components
- Prefer Signal Forms (`@angular/forms/signals`) for new forms. They are stable in Angular v22+ and provide signal-based state, type-safe field access, and schema-based validation
- When not using Signal Forms, prefer Reactive forms instead of Template-driven ones
- Do NOT use `ngClass`, use `class` bindings instead
- Do NOT use `ngStyle`, use `style` bindings instead
- Do NOT import `CommonModule`, import only the directives and pipes the template uses, such as `AsyncPipe` or `DatePipe`
- When using external templates/styles, use paths relative to the component TS file.

## State Management

- Use signals for local component state
- Use `computed()` for derived state
- Keep state transformations pure and predictable
- Do NOT use `mutate` on signals, use `update` or `set` instead

## Templates

- Keep templates simple and avoid complex logic
- Use native control flow (`@if`, `@for`, `@switch`) instead of `*ngIf`, `*ngFor`, `*ngSwitch`
- Use the async pipe to handle observables
- Do not assume globals like (`new Date()`) are available.

## Services

- Design services around a single responsibility
- Use the `providedIn: 'root'` option for singleton services
- Prefer the `@Service` decorator over `@Injectable({providedIn: 'root'})` for new singleton services (Angular v22+)
- Use the `inject()` function instead of constructor injection

## Project UI Stack (see ADRs in `../docs/adr/`)

- Styling: Tailwind CSS v4 via `@tailwindcss/postcss`. Plain CSS only — do NOT use SCSS/Sass. Design tokens live in `projects/shop/src/styles.css`. ([ADR-0016](../docs/adr/0016-styling-i-komponenty-ui.md))
- UI components: spartan/ui only. Add components with the spartan CLI (`npx ng g @spartan-ng/cli:ui`); they land in `libs/ui` (import alias `@spartan-ng/helm`) and are our code — change their look there. ([ADR-0016](../docs/adr/0016-styling-i-komponenty-ui.md))
- Behavior and a11y: Angular CDK (overlay, scrolling, a11y, drag-drop). ([ADR-0016](../docs/adr/0016-styling-i-komponenty-ui.md))
- Charts: `ngx-echarts`. Tables: spartan/ui table; add `@tanstack/angular-table` only when the simple table is not enough. ([ADR-0012](../docs/adr/0012-dashboard.md), [ADR-0017](../docs/adr/0017-wykresy-i-tabele-panelu-admina.md))
- Icons: `@lucide/angular`, import only the icons you use. ([ADR-0018](../docs/adr/0018-ikony-i-przejscia-widokow.md))
- Route transitions: `withViewTransitions()` in the router, animations in CSS (`::view-transition-*`), respect `prefers-reduced-motion`. ([ADR-0018](../docs/adr/0018-ikony-i-przejscia-widokow.md))

## Forbidden

- Do NOT install PrimeNG, `@angular/material`, Sass, or any other component or animation library. Changing this requires a new ADR.
- The tracker library must not import Tailwind, spartan/ui, CDK, or any shop code. ([ADR-0006](../docs/adr/0006-schemat-eventu-i-tracker.md))

## Formatting

- Prettier with `prettier-plugin-tailwindcss` sorts Tailwind classes. Do not sort them by hand.
