import axios from "axios";
import { BaseService } from "./baseService";
import { GoodsReceipt, GoodsReceiptRegister } from '@/interfaces/goodsReceiptInterface'
import { BaseResponse } from '@/interfaces/baseInterface';

class GoodsReceiptService extends BaseService<GoodsReceipt> {
  constructor() {
    super({
      endpoint: 'GoodsReceipt',
      downloadFileName: 'Entradas',
      customEndpoints: {
        create: 'GoodsReceipt/Register'
      }
    });
  }

  // Registrar nueva entrada de productos
  async register(data: GoodsReceiptRegister): Promise<BaseResponse<any>> {
    const response = await axios.post<BaseResponse<any>>('api/GoodsReceipt/Register', data);
    return response.data;
  }

  // Anular una entrada de productos
  async disable(receiptId: number): Promise<BaseResponse<void>> {
    const response = await axios.put<BaseResponse<void>>(`api/GoodsReceipt/Disable/${receiptId}`, {});
    return response.data;
  }

  // Obtener detalles de una entrada específica
  async getReceiptWithDetails(receiptId: number): Promise<BaseResponse<any>> {
    const response = await axios.get<BaseResponse<any>>(
      `api/GoodsReceipt/${receiptId}`
    );
    return response.data;
  }

  // Exportar PDF de una entrada
  async exportPdf(receiptId: number): Promise<{ blob: Blob; filename: string }> {
    const response = await axios.get(
      `api/GoodsReceipt/ExportPdf/${receiptId}`,
      { responseType: 'blob' }
    );

    // Extraer el nombre del archivo del header Content-Disposition
    const contentDisposition =
      response.headers['content-disposition'] ||
      response.headers['Content-Disposition'] ||
      response.headers.contentDisposition;

    let filename = `Entrada_${receiptId}.pdf`; // Nombre por defecto

    if (contentDisposition) {
      // Primero intenta extraer filename*=UTF-8''
      const utf8Match = contentDisposition.match(/filename\*=UTF-8''([^;]+)/i);

      if (utf8Match && utf8Match[1]) {
        filename = decodeURIComponent(utf8Match[1]);
      } else {
        // Si no encuentra UTF-8, intenta con filename=
        const standardMatch = contentDisposition.match(/filename[^;=\n]*=["']?([^"';\n]+)["']?/i);

        if (standardMatch && standardMatch[1]) {
          filename = standardMatch[1].replace(/^["']|["']$/g, '');
        }
      }
    }

    return {
      blob: response.data,
      filename: filename
    };
  }
}

export const goodsReceiptService = new GoodsReceiptService();