<script setup lang="ts">
import { ref } from 'vue';
import ContactForm from './components/ContactForm.vue';
import OrderForm from './components/OrderForm.vue';
import SubmissionsList from './components/SubmissionsList.vue';

const activeTab = ref<'contact' | 'order' | 'submissions'>('contact');
</script>

<template>
  <div class="app-container">
    <header>
      <h1>Form Submission System</h1>
      <p class="subtitle">Demonstrating dynamic form handling with Vue.js & ASP.NET Core</p>
      <nav class="tabs">
        <button
          @click="activeTab = 'contact'"
          :class="{ active: activeTab === 'contact' }"
          class="tab-button"
        >
          Contact Form
        </button>
        <button
          @click="activeTab = 'order'"
          :class="{ active: activeTab === 'order' }"
          class="tab-button"
        >
          Order Form
        </button>
        <button
          @click="activeTab = 'submissions'"
          :class="{ active: activeTab === 'submissions' }"
          class="tab-button"
        >
          View Submissions
        </button>
      </nav>
    </header>

    <main>
      <ContactForm v-if="activeTab === 'contact'" />
      <OrderForm v-else-if="activeTab === 'order'" />
      <SubmissionsList v-else />
    </main>
  </div>
</template>

<style>
* {
  margin: 0;
  padding: 0;
  box-sizing: border-box;
}

body {
  font-family: -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, Oxygen, Ubuntu, Cantarell, 'Helvetica Neue', sans-serif;
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
  min-height: 100vh;
}

#app {
  min-height: 100vh;
  padding: 2rem 1rem;
}
</style>

<style scoped>
.app-container {
  max-width: 1200px;
  margin: 0 auto;
}

header {
  text-align: center;
  margin-bottom: 2rem;
}

h1 {
  color: white;
  font-size: 2.5rem;
  margin-bottom: 0.5rem;
  text-shadow: 2px 2px 4px rgba(0, 0, 0, 0.2);
}

.subtitle {
  color: rgba(255, 255, 255, 0.9);
  font-size: 1rem;
  margin-bottom: 1.5rem;
}

.tabs {
  display: flex;
  justify-content: center;
  gap: 1rem;
  flex-wrap: wrap;
}

.tab-button {
  padding: 0.75rem 2rem;
  background: rgba(255, 255, 255, 0.2);
  color: white;
  border: 2px solid white;
  border-radius: 25px;
  font-size: 1rem;
  font-weight: 600;
  cursor: pointer;
  transition: all 0.3s;
}

.tab-button:hover {
  background: rgba(255, 255, 255, 0.3);
  transform: translateY(-2px);
}

.tab-button.active {
  background: white;
  color: #667eea;
}

main {
  margin-top: 2rem;
}
</style>
