<template>
  <div id="app">
    <DrawerDemo />
  </div>
</template>

<script>
import DrawerDemo from "./components/DrawerDemo.vue";

export default {
  name: "App",
  components: {
    DrawerDemo,
  },
    mounted() {
    document.title = "UShortSharp"
  }
};
</script>

<style>
#app {
  font-family: "Segoe UI", Tahoma, Geneva, Verdana, Arial, Helvetica, sans-serif;
  -webkit-font-smoothing: antialiased;
  -moz-osx-font-smoothing: grayscale;
  color: #2c3e50;
  /* margin-top: 0px; */

  
}

body {
  background-image: url('./assets/sunset.jpg');
  background-position: center;
  background-size: cover;
  /* background-color: grey; */
  overflow: hidden;
  backdrop-filter: blur(8px);
  margin: 0;
  /* filter: blur(8px); */
  /* -webkit-filter: blur(2px); */


}
</style>
