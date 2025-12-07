export interface Trade {
  id: number;
  security: string;
  side: 'Buy' | 'Sell';
  quantity: number;  
  version: number;
}