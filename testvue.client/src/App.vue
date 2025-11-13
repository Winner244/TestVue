<script setup lang="ts">
import { computed } from 'vue';
import { useRoute } from 'vue-router';
import Header from './components/Header/Header.vue';
import './assets/styles/app.less'

const route = useRoute();
const activeTab = computed(() => route.name === 'submissions' ? 'submissions' : 'contact');
</script>

<template>
    <div class="app-container">
        <Header :activeTab="activeTab" />

        <main role="main" aria-label="Main content">
            <router-view />
        </main>
    </div>
</template>
