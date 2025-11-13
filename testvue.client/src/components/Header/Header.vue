<template>
    <header class="header">
        <h1 class="header__title">Test ASP.NET + Vue</h1>
        <nav class="header__nav" aria-label="Main navigation">
            <router-link to="/"
                :class="['header__tab', { 'header__tab--active': activeTab === 'contact' }]"
                :aria-current="activeTab === 'contact' ? 'page' : undefined">
                Contact Form
            </router-link>

            <router-link to="/submissions"
                :class="['header__tab', { 'header__tab--active': activeTab === 'submissions' }]"
                :aria-current="activeTab === 'submissions' ? 'page' : undefined">
                View Result
            </router-link>
        </nav>
    </header>
</template>

<style scoped lang="less" src="./header.less"></style>

<script setup lang="ts">
    defineProps<{
        activeTab: 'contact' | 'submissions';
    }>();
</script>
