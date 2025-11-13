# Vue 3 + TypeScript + ASP.NET Core

## 🚀 Features

- ✅ **Vue 3 Composition API** - Modern reactive state management with `<script setup>`
- ✅ **TypeScript** - Full type safety across the application
- ✅ **Zod Validation** - Runtime type-safe schema validation
- ✅ **Accessibility** - WCAG compliant with ARIA labels and semantic HTML
- ✅ **Testing** - Comprehensive test suite with Vitest and Vue Test Utils
- ✅ **BEM Methodology** - Maintainable and scalable CSS architecture
- ✅ **Composables Pattern** - Reusable logic with custom composables
- ✅ **Toast Notifications** - Non-intrusive user feedback
- ✅ **Environment Configuration** - Separate dev/prod configurations

## 📋 Prerequisites

- Node.js: `^20.19.0` or `>=22.12.0`
- npm or yarn
- ASP.NET Core 8.0 (for backend API)

## 🛠️ Installation

1. **Clone the repository**
   ```bash
   git clone <repository-url>
   cd TestVue/testvue.client
   ```

2. **Install dependencies**
   ```bash
   npm install
   ```

3. **Environment Setup**
   
   Environment variables are in `.env`, `.env.development`, and `.env.production` files.

## 🎯 Available Scripts

### Development
```bash
npm run dev          # Start development server with hot-reload
npm run build        # Build for production
npm run preview      # Preview production build locally
```

### Code Quality
```bash
npm run lint         # Run ESLint with auto-fix
npm run type-check   # Run TypeScript type checking
```

### Testing
```bash
npm test             # Run tests in watch mode
npm run test:ui      # Open Vitest UI for interactive testing
npm run test:coverage # Generate coverage report
```


## 🌐 API Integration

- `POST /api/formsubmission` - Submit form data
- `GET /api/formsubmission` - Fetch all submissions

API base URL configured via environment variables in `src/config/env.ts`.

## 🎯 Best Practices Implemented

1. Composition API with `<script setup>`
2. TypeScript type safety
3. Zod runtime validation
4. Composables for code reuse
5. Lazy loading for performance
6. Debouncing for optimization
7. loaders for UX
8. WCAG accessibility compliance
9. BEM CSS methodology
10. Comprehensive testing
11. Environment configuration
12. Toast notifications
13. Error handling
14. Semantic HTML

## 🚀 Performance Optimizations

- Code splitting with dynamic imports
- Lazy loading with `defineAsyncComponent`
- Debounced search (300ms)
- Computed properties for expensive operations
- Suspense for async components
- Skeleton loaders

## Recommended IDE Setup

[VS Code](https://code.visualstudio.com/) + [Vue (Official)](https://marketplace.visualstudio.com/items?itemName=Vue.volar) + [TypeScript Vue Plugin (Volar)](https://marketplace.visualstudio.com/items?itemName=Vue.vscode-typescript-vue-plugin)

## Recommended Browser Extensions

- **Chrome/Edge**: [Vue.js devtools](https://chromewebstore.google.com/detail/vuejs-devtools/nhdogjmejiglipccpnnnanhbledajbpd)
- **Firefox**: [Vue.js devtools](https://addons.mozilla.org/en-US/firefox/addon/vue-js-devtools/)

---

**Built with** ❤️ **using Vue 3, TypeScript, and Modern Web Standards**
