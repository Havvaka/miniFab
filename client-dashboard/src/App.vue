<template>
  <div id="app">
    <header>
      <img alt="Vue logo" src="./assets/logo.png" width="60">
      <h1>Sensör İzleme Sistemi</h1>
    </header>
    
    <main>
      <Dashboard deviceId="DEVICE-001" :limit="10" />
    </main>
    
    <footer>
      <p>&copy; 2025 MiniFab Sensör İzleme</p>
    </footer>
  </div>
</template>

<script>
import Dashboard from './views/Dashboard.vue'

export default {
  name: 'App',
  components: {
    Dashboard
  }
}
</script>

<style>
#app {
  font-family: Avenir, Helvetica, Arial, sans-serif;
  -webkit-font-smoothing: antialiased;
  -moz-osx-font-smoothing: grayscale;
  text-align: center;
  color: #2c3e50;
  max-width: 1200px;
  margin: 0 auto;
  padding: 20px;
}

header {
  display: flex;
  align-items: center;
  justify-content: center;
  margin-bottom: 30px;
  border-bottom: 1px solid #eaeaea;
  padding-bottom: 20px;
}

header img {
  margin-right: 15px;
}

header h1 {
  margin: 0;
  font-size: 1.8rem;
}

main {
  min-height: 70vh;
}

footer {
  margin-top: 40px;
  padding-top: 20px;
  border-top: 1px solid #eaeaea;
  color: #6c757d;
  font-size: 0.9rem;
}
</style>