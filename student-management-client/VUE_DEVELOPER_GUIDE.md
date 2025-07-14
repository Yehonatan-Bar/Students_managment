# Vue 3 Developer Guide for Non-Vue Developers

This guide explains the Vue.js concepts used in this project for developers coming from other frameworks.

## Project Structure

```
student-management-client/
├── src/
│   ├── components/         # Vue components (UI building blocks)
│   ├── services/          # API and external service integrations
│   ├── types/             # TypeScript type definitions
│   ├── utils/             # Utility functions and helpers
│   ├── App.vue            # Root component
│   └── main.ts            # Application entry point
├── .env                   # Environment variables
└── package.json           # Dependencies and scripts
```

## Key Vue Concepts

### 1. Single File Components (.vue files)

Vue components combine template (HTML), script (JS/TS), and style (CSS) in one file:

```vue
<template>
  <!-- HTML structure -->
</template>

<script setup lang="ts">
  // JavaScript/TypeScript logic
</script>

<style scoped>
  /* Component-specific styles */
</style>
```

### 2. Reactive Data with `ref()`

```typescript
const count = ref(0)         // Creates reactive variable
count.value = 1             // Update in script
{{ count }}                 // Use in template (no .value needed)
```

### 3. Template Directives

- `v-if` / `v-else-if` / `v-else` - Conditional rendering
- `v-for` - List rendering
- `@click` - Event handling (shorthand for `v-on:click`)
- `:prop` - Dynamic props/attributes (shorthand for `v-bind:prop`)
- `{{ expression }}` - Text interpolation

### 4. Lifecycle Hooks

- `onMounted()` - Runs after component is added to DOM (like React's useEffect with [])
- `onUnmounted()` - Cleanup when component is removed

### 5. Computed Properties

```typescript
const doubled = computed(() => count.value * 2)
```
Automatically updates when dependencies change.

## Environment Variables

- Must be prefixed with `VITE_` to be accessible in the client
- Access via `import.meta.env.VITE_VARIABLE_NAME`
- Defined in `.env` file

## Running the Project

```bash
npm install          # Install dependencies
npm run dev         # Start development server
npm run build       # Build for production
```

## Common Patterns in This Project

### API Service Pattern
- Centralized API calls in `services/api.ts`
- Axios interceptors for logging
- Async/await for clean asynchronous code

### Logging System
- Dual-tag structure (feature + module)
- Comprehensive parameter tracking
- Development-only console output

### Virtual Scrolling
- Performance optimization for large lists
- Only renders visible items
- Maintains smooth scrolling experience

## Debugging Tips

1. **Vue DevTools** - Browser extension for inspecting Vue components
2. **Check Console** - Logger outputs detailed information in development
3. **Network Tab** - Monitor API calls and responses
4. **TypeScript Errors** - VSCode provides real-time type checking

## Common Gotchas

1. **Reactivity** - Remember to use `.value` when accessing ref values in script
2. **Async Setup** - Can't use `await` at top level of `<script setup>`
3. **Props are Readonly** - Don't modify props directly, use emit or local state
4. **Template Expressions** - Keep them simple, use computed for complex logic

## Comparison to Other Frameworks

### If you know React:
- `ref()` is like `useState()`
- `computed()` is like `useMemo()`
- `onMounted()` is like `useEffect(() => {}, [])`
- Components are similar but combine template/logic/style

### If you know Angular:
- `.vue` files are like Angular components
- Directives (`v-if`, `v-for`) similar to Angular (`*ngIf`, `*ngFor`)
- Services pattern is similar (dependency injection not built-in)
- Two-way binding possible with `v-model`

## Resources

- [Vue 3 Documentation](https://vuejs.org/)
- [Vite Documentation](https://vitejs.dev/)
- [TypeScript with Vue](https://vuejs.org/guide/typescript/overview.html)
- [Vue Composition API](https://vuejs.org/guide/extras/composition-api-faq.html)