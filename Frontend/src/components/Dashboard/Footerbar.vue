<template>
    <v-footer app>
        <v-card class="footer-custom px-4 py-2 text-center w-100" flat>
            <v-card-text class="py-2 white--text text-center">
                <strong>{{ new Date().getFullYear() }}</strong> - <strong>IT SOLUTIONS</strong>
            </v-card-text>
        </v-card>
    </v-footer>
</template>

<style scoped>
.footer-custom {
  background-color: rgb(26, 32, 44);
  color: white; 
}
</style>