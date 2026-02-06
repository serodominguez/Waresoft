import axios from "axios";
import { BaseService } from "./baseService";
import { GoodsIssue, GoodsIssueRegister } from '@/interfaces/goodsIssueInterface'
import { BaseResponse } from '@/interfaces/baseInterface';

class GoodsIssueService extends BaseService<GoodsIssue> {
  constructor() {
    super({
      endpoint: 'GoodsIssue',
      downloadFileName: 'Salidas',
      customEndpoints: {
        create: 'GoodsIssue/Register'
      }
    });
  }

  async register(data: GoodsIssueRegister): Promise<BaseResponse<any>> {
    const response = await axios.post<BaseResponse<any>>('api/GoodsIssue/Register', data);
    return response.data;
  }

  async disable(issueId: number): Promise<BaseResponse<void>> {
    const response = await axios.put<BaseResponse<void>>(`api/GoodsIssue/Disable/${issueId}`, {});
    return response.data;
  }

  async getIssueWithDetails(issueId: number): Promise<BaseResponse<any>> {
    const response = await axios.get<BaseResponse<any>>(
      `api/GoodsIssue/${issueId}`
    );
    return response.data;
  }

  async exportPdf(issueId: number): Promise<{ blob: Blob; filename: string }> {
    const response = await axios.get(
      `api/GoodsIssue/ExportPdf/${issueId}`,
      { responseType: 'blob' }
    );
    
    const contentDisposition =
      response.headers['content-disposition'] ||
      response.headers['Content-Disposition'] ||
      response.headers.contentDisposition;

    let filename = `Salida_${issueId}.pdf`;

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

export const goodsIssueService = new GoodsIssueService();