import axios from "axios";
import { BaseService } from "./baseService";
import { Transfer, TransferRegister } from '@/interfaces/transferInterface'
import { BaseResponse } from '@/interfaces/baseInterface';

class TransferService extends BaseService<Transfer> {
  constructor() {
    super({
      endpoint: 'Transfer',
      downloadFileName: 'Traspaso',
      customEndpoints: {
        create: 'Transfer/Send'
      }
    });
  }

  async send(data: TransferRegister): Promise<BaseResponse<any>> {
    const response = await axios.post<BaseResponse<any>>('api/Transfer/Send', data);
    return response.data;
  }

  async receive(transferId: number): Promise<BaseResponse<void>> {
    const response = await axios.put<BaseResponse<void>>(`api/Transfer/Receive/${transferId}`, {});
    return response.data;
  }

  async disable(transferId: number): Promise<BaseResponse<void>> {
    console.log("hola");
    const response = await axios.put<BaseResponse<void>>(`api/Transfer/Disable/${transferId}`, {});
    return response.data;
  }

  async getTransferWithDetails(transferId: number): Promise<BaseResponse<any>> {
    const response = await axios.get<BaseResponse<any>>(
      `api/Transfer/${transferId}`
    );
    return response.data;
  }

  async exportPdf(transferId: number): Promise<{ blob: Blob; filename: string }> {
    const response = await axios.get(
      `api/Transfer/ExportPdf/${transferId}`,
      { responseType: 'blob' }
    );
    
    const contentDisposition =
      response.headers['content-disposition'] ||
      response.headers['Content-Disposition'] ||
      response.headers.contentDisposition;

    let filename = `Traspaso_${transferId}.pdf`;

    if (contentDisposition) {
      const utf8Match = contentDisposition.match(/filename\*=UTF-8''([^;]+)/i);

      if (utf8Match && utf8Match[1]) {
        filename = decodeURIComponent(utf8Match[1]);
      } else {
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

export const transferService = new TransferService();