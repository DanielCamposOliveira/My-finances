import { Component,inject } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import {
  MAT_SNACK_BAR_DATA,
  MatSnackBarAction,
  MatSnackBarActions,
  MatSnackBarLabel,
  MatSnackBarRef,
} from '@angular/material/snack-bar';


export interface SnackBarConfigData {
  mensagem: string;
  icone?: string;
}

@Component({
  selector: 'app-snack-bar-info',
  standalone: true,
  imports: [MatButtonModule, MatSnackBarLabel, MatSnackBarActions, MatSnackBarAction],
  templateUrl: './snack-bar-info.html',
  styleUrl: './snack-bar-info.scss',
})
export class SnackBarCustomComponent {
  readonly snackBarRef = inject(MatSnackBarRef);
  readonly data: SnackBarConfigData = inject(MAT_SNACK_BAR_DATA);
}
