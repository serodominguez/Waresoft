import { useBrandStore } from '@/stores/brandStore';
import { useCategoryStore } from '@/stores/categoryStore';
import { useCustomerStore } from '@/stores/customerStore';
import { useGoodsIssueStore } from '@/stores/goodsIssueStore';
import { useGoodsReceiptStore } from '@/stores/goodsReceiptStore';
import { useModuleStore } from '@/stores/moduleStore';
import { useProductStore } from '@/stores/productStore';
import { useRoleStore } from '@/stores/roleStore';
import { useStoreStore } from '@/stores/storeStore';
import { useSupplierStore } from '@/stores/supplierStore';
import { useTransferStore } from '@/stores/transferStore'
import { useUserStore } from '@/stores/userStore';
// ... importa los demás stores

export const useStoreMapper = () => {
    // Mapa de stores
    const storeMap: Record<string, any> = {
        brand: useBrandStore(),
        category: useCategoryStore(),
        customer: useCustomerStore(),
        goodsissue: useGoodsIssueStore(),
        goodsreceipt: useGoodsReceiptStore(),
        module: useModuleStore(),
        product: useProductStore(),
        role: useRoleStore(),
        store: useStoreStore(),
        supplier: useSupplierStore(),
        transfer: useTransferStore(),
        user: useUserStore(),
        // ... agrega los demás módulos
    };

    /**
     * Obtiene la acción correspondiente del store
     * @param moduleName - Nombre del módulo (ej: 'brand', 'category')
     * @param actionType - Tipo de acción ('eliminar', 'activa', 'inactiva', 'cancelar')
     * @param entityName - Nombre de la entidad (ej: 'Brand', 'Category')
     */
    const getStoreAction = (
        moduleName: string,
        actionType: 'eliminar' | 'activar' | 'inactivar' | 'cancelar',
        entityName: string
    ) => {
        const store = storeMap[moduleName];

        if (!store) {
            throw new Error(`Store no encontrado para el módulo: ${moduleName}`);
        }

        // Mapeo de acción a prefijo del método
        const actionPrefix: Record<string, string> = {
            eliminar: 'remove',
            activar: 'enable',
            inactivar: 'disable',
            cancelar: 'disable'
        };

        // Construir el nombre del método (ej: 'removeBrand', 'enableCategory')
        const methodName = `${actionPrefix[actionType]}${entityName}`;

        if (typeof store[methodName] !== 'function') {
            throw new Error(
                `Método ${methodName} no encontrado en el store ${moduleName}`
            );
        }

        return store[methodName].bind(store);
    };

    return {
        getStoreAction
    };
};