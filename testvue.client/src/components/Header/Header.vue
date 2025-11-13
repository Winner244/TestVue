<template>
    <header class="header">
        <h1 class="header__title">Test ASP.NET + Vue</h1>
        <nav class="header__nav">
            <button @click="handleTabClick('contact')"
                :class="['header__tab', { 'header__tab--active': activeTab === 'contact' }]">
                Contact Form
            </button>

            <button @click="handleTabClick('submissions')"
                :class="['header__tab', { 'header__tab--active': activeTab === 'submissions' }]">
                View Result
            </button>
        </nav>
    </header>
</template>

<style scoped lang="less" src="./header.less"></style>

<script setup lang="ts">
    defineProps<{
        activeTab: 'contact' | 'submissions';
    }>();

    const emit = defineEmits<{
        (e: 'update:activeTab', value: 'contact' | 'submissions'): void;
    }>();

    const handleTabClick = (tab: 'contact' | 'submissions') => {
        emit('update:activeTab', tab);
    };
</script>
