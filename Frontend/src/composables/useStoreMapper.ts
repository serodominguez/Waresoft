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
import { useTransferStore } from '@/stores/transferStore';
import { useUserStore } from '@/stores/userStore';

//Mapa de stores
export type StoreMap = {
    brand:        ReturnType<typeof useBrandStore>;
    category:     ReturnType<typeof useCategoryStore>;
    customer:     ReturnType<typeof useCustomerStore>;
    goodsissue:   ReturnType<typeof useGoodsIssueStore>;
    goodsreceipt: ReturnType<typeof useGoodsReceiptStore>;
    module:       ReturnType<typeof useModuleStore>;
    product:      ReturnType<typeof useProductStore>;
    role:         ReturnType<typeof useRoleStore>;
    store:        ReturnType<typeof useStoreStore>;
    supplier:     ReturnType<typeof useSupplierStore>;
    transfer:     ReturnType<typeof useTransferStore>;
    user:         ReturnType<typeof useUserStore>;
};

type StoreActions = {
    remove:  (id: number) => Promise<{ isSuccess: boolean }>;
    enable:  (id: number) => Promise<{ isSuccess: boolean }>;
    disable: (id: number) => Promise<{ isSuccess: boolean }>;
    cancel:  (id: number) => Promise<{ isSuccess: boolean }>;
};

//Acciones
const ACTION_PREFIX = {
    eliminar:  'remove',
    activar:   'enable',
    inactivar: 'disable',
    cancelar:  'cancel',
} as const;

type ActionType = 'eliminar' | 'activar' | 'inactivar' | 'cancelar';
type PrefixMap = typeof ACTION_PREFIX;

//Composable
export const useStoreMapper = () => {
    const storeMap: StoreMap = {
        brand:        useBrandStore(),
        category:     useCategoryStore(),
        customer:     useCustomerStore(),
        goodsissue:   useGoodsIssueStore(),
        goodsreceipt: useGoodsReceiptStore(),
        module:       useModuleStore(),
        product:      useProductStore(),
        role:         useRoleStore(),
        store:        useStoreStore(),
        supplier:     useSupplierStore(),
        transfer:     useTransferStore(),
        user:         useUserStore(),
    };

    /**
     * Obtiene la acción correspondiente del store con tipo completamente inferido.
     *
     * @param moduleName - Clave del módulo ('brand', 'category', ...)
     * @param actionType - Tipo de acción ('eliminar' | 'activar' | 'inactivar' | 'cancelar')
     * @param entityName - Nombre de la entidad en PascalCase ('Brand', 'Category', ...)
     *
     * @returns La función del store ligada (bound) con su firma original preservada.
     *
     * @example
     * const fn = getStoreAction('brand', 'eliminar', 'Brand');
     * // typeof fn → (id: string) => Promise<void>
     */
function getStoreAction(
    moduleName: keyof StoreMap,
    actionType: ActionType,
): (id: number) => Promise<{ isSuccess: boolean }> {
    const store      = storeMap[moduleName] as unknown as StoreActions;
    const methodName = ACTION_PREFIX[actionType];

    if (typeof store[methodName] !== 'function') {
        throw new Error(
            `Método "${methodName}" no encontrado en el store "${moduleName}"`
        );
    }

    return store[methodName].bind(store as unknown as object);
}

    return { getStoreAction };
};