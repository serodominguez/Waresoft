/**
 * Archivo de declaración de tipos para TypeScript
 * La exportación vacía convierte este archivo en un módulo ES6
 * Esto es necesario para que las declaraciones de 'declare module' funcionen correctamente
 */
export {}

declare module '@vue/runtime-core' {
  interface ComponentCustomProperties {
    $hasPermission: (module: string, action: string) => boolean
  }
}