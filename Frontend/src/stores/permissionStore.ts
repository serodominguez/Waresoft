import { defineStore } from 'pinia'
import { fetchPermissionsByRole, updatePermissions } from '@/services/permissionService'
import { Permission, PermissionsByModule } from '@/interfaces/permissionInterface'
import { handleSilentError } from '@/helpers/errorHandler'

interface PermissionState {
  permissions: Permission[]
  loading: boolean
  error: string | null
}

export const usePermissionStore = defineStore('permission', {
  state: (): PermissionState => ({
    permissions: [],
    loading: false,
    error: null,
  }),

  getters: {
    permissionsByModule: (state): PermissionsByModule[] => {
      const grouped = state.permissions.reduce((acc, perm) => {
        if (!acc[perm.moduleName]) {
          acc[perm.moduleName] = {
            module: perm.moduleName,
            permissions: {
              crear: false,
              leer: false,
              editar: false,
              eliminar: false,
              descargar: false
            },
          }
        }

        const actionKey = perm.actionName.toLowerCase() as
          | 'crear'
          | 'leer'
          | 'editar'
          | 'eliminar'
          | 'descargar'
        acc[perm.moduleName].permissions[actionKey] = perm.status

        return acc
      }, {} as Record<string, PermissionsByModule>)

      return Object.values(grouped)
    },
  },

  actions: {
    async fetchPermissionsByRole(roleId: number) {
      this.loading = true
      this.error = null
      try {
        const response = await fetchPermissionsByRole(roleId)
        if (response.isSuccess) {
          this.permissions = response.data
        } else {
          this.error = response.message
        }
      } catch (error: any) {
        const appError = handleSilentError(error)
        this.error = appError.message
        throw error
      } finally {
        this.loading = false
      }
    },

    async updatePermissions(updatedPermissions: Array<{ idPermission: number; status: boolean }>) {
      try {
        const result = await updatePermissions(updatedPermissions)
        if (result.isSuccess) {
          return { success: true, message: result.message }
        } else {
          this.error = result.message || result.errors
          return {
            success: false,
            message: result.message || 'Error al actualizar permisos',
          }
        }
      } catch (error: any) {
        const appError = handleSilentError(error)
        this.error = appError.message
        throw error
      }
    },
  },
})