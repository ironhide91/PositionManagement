import { Component, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { SignalRService } from './services/signalr.service';
import { Transaction } from './models/transaction.model';

@Component({
  selector: 'app-root',
  imports: [CommonModule, FormsModule],
  templateUrl: './app.html',
  styleUrl: './app.css'
})
export class App {
  protected readonly title = signal('position-webui');
  
  // Form model
  protected txForm = signal<Transaction>({
    id: 0,
    tradeId: 0,
    version: 0,
    security: '',
    quantity: 0,
    action: 'Insert',
    side: 'Buy',
  });

  protected editMode = signal(false);
  protected selectedTxId = signal<number | null>(null);

  constructor(protected signalRService: SignalRService) {}
  
  protected setSide(side: 'Buy' | 'Sell'): void {
    this.txForm.update(form => ({ 
      ...form, 
      side: side 
    }));
  }

  protected async onSubmit(): Promise<void> {
    const form = this.txForm();
    if (!form.security || !form.quantity) {
      alert('Please fill all fields');
      return;
    }

    try {
      if (this.editMode() && this.selectedTxId() !== null) {
        await this.signalRService.updateTransaction({ 
          ...form, 
          id: this.selectedTxId()! 
        });
      } else {
        await this.signalRService.insertTransaction(form);
      }
      this.resetForm();
    } catch (err) {
      console.error('Error submitting transaction:', err);
      alert('Error submitting transaction');
    }
  }

  protected editTransaction(tx: Transaction): void {
    this.editMode.set(true);
    this.selectedTxId.set(tx.id);
    this.txForm.set({ ...tx });
  }

  protected async cancelTransaction(tx: Transaction): Promise<void> {
    if (confirm(`Are you sure you want to cancel transaction ${tx.id}?`)) {
      try {
        await this.signalRService.cancelTransaction(tx);
      } catch (err) {
        console.error('Error canceling transaction:', err);
        alert('Error canceling transaction');
      }
    }
  }

  protected resetForm(): void {
    this.txForm.set({
		id: 0,
		tradeId: 0,
		version: 0,
		security: '',
		quantity: 0,
		action: 'Insert',
		side: 'Buy',
    });
    this.editMode.set(false);
    this.selectedTxId.set(null);
  }

  protected updateFormField(field: keyof Transaction, value: any): void {
    this.txForm.update(form => ({ ...form, [field]: value }));
  }
}
