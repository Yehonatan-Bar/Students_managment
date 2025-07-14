<!-- 
  App.vue is the root component of the Vue application
  In Vue, the entire app is built as a tree of components
  This is the top-level component that gets mounted to the DOM
-->

<script setup lang="ts">
// Import the StudentListSimple component
// Using the simple version - virtual scrolling had compatibility issues
import StudentListSimple from './components/StudentListSimple.vue'
</script>

<template>
  <!-- 
    The template section contains the HTML structure
    Here we're simply rendering the StudentListSimple component
    Vue components are used like custom HTML tags
  -->
  <StudentListSimple />
</template>

<style scoped>
/* 
  Global styles for the app can go here
  The 'scoped' attribute means these styles only apply to this component
  Currently empty as all styling is handled in child components
*/
</style>
