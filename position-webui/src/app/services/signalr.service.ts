import { Injectable, signal } from '@angular/core';
import * as signalR from '@microsoft/signalr';
import { Transaction } from '../models/transaction.model';
import { Position } from '../models/position.model';
import { Trade } from '../models/trade.model';

@Injectable({
  providedIn: 'root'
})
export class SignalRService {
  private hubConnection!: signalR.HubConnection;
  
  public transactions = signal<Transaction[]>([]);
  public positions = signal<Position[]>([]);
  public trades = signal<Trade[]>([]);
  public isConnected = signal<boolean>(false);

  constructor() {
    this.startConnection();
  }

  private startConnection(): void {
    this.hubConnection = new signalR.HubConnectionBuilder()
      .withUrl('http://localhost:5069/signalrhub')
      .withAutomaticReconnect()
      .build();

    this.hubConnection
      .start()
      .then(() => {
        console.log('SignalR Connected');
        this.isConnected.set(true);
        this.loadInitialData();
        this.registerHandlers();
      })
      .catch(err => {
        console.error('Error connecting to SignalR:', err);
        this.isConnected.set(false);
      });

    this.hubConnection.onreconnected(() => {
      console.log('SignalR Reconnected');
      this.isConnected.set(true);
      this.loadInitialData();
    });

    this.hubConnection.onreconnecting(() => {
      console.log('SignalR Reconnecting...');
      this.isConnected.set(false);
    });

    this.hubConnection.onclose(() => {
      console.log('SignalR Disconnected');
      this.isConnected.set(false);
    });
  }

  private registerHandlers(): void {
    this.hubConnection.on('TransactionsUpdated', (transactions: Transaction[]) => {
      this.transactions.set(transactions);
    });

    this.hubConnection.on('PositionsUpdated', (positions: Position[]) => {
      this.positions.set(positions);
    });
	
	this.hubConnection.on('TradesUpdated', (trades: Trade[]) => {
      this.trades.set(trades);
    });
  }

  private async loadInitialData(): Promise<void> {
    try {
      const [transactions, positions] = await Promise.all([
        this.hubConnection.invoke<Transaction[]>('GetTransactionsAsync'),
        this.hubConnection.invoke<Position[]>('GetPositionsAsync'),
        this.hubConnection.invoke<Trade[]>('GetTradesAsync'),
      ]);
      this.transactions.set(transactions);
      this.positions.set(positions);
    } catch (err) {
      console.error('Error loading initial data:', err);
    }
  }

  public async insertTransaction(tx: Transaction): Promise<void> {
    try {
      await this.hubConnection.invoke('InsertAsync', tx);
    } catch (err) {
      console.error('Error inserting transaction:', err);
      throw err;
    }
  }

  public async updateTransaction(tx: Transaction): Promise<void> {
    try {
      await this.hubConnection.invoke('UpdateAsync', tx);
    } catch (err) {
      console.error('Error updating transaction:', err);
      throw err;
    }
  }

  public async cancelTransaction(tx: Transaction): Promise<void> {
    try {
      await this.hubConnection.invoke('CancelAsync', tx);
    } catch (err) {
      console.error('Error canceling transaction:', err);
      throw err;
    }
  }
}
