export interface Transaction {
  id: number;
  tradeId: number;
  version: number;
  security: string;
  quantity: number;
  action: 'Insert' | 'Update' | 'Cancel';
  side: 'Buy' | 'Sell';
}