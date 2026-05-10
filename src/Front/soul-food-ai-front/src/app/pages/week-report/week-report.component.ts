import { Component, OnInit, ChangeDetectorRef,NgZone } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { WeekReportService } from '../../services/week-report.service';
import { UserService } from '../../services/user.service';

@Component({
  selector: 'app-week-report',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './week-report.component.html',
  styleUrls: ['./week-report.component.css']
})
export class WeekReportComponent implements OnInit {
  step: number = 1; 
  
  report = {
    idUser: 0,
    hungerLevel: 5,
    sleepQuality: 5,
    energyLevel: 5,
    dietAdherence: 5,
    description: '',
    newWeight: null,
    newMeasures: {
      chestMeasure: 0, waistMeasure: 0, hipMeasure: 0,
      leftBicepMeasure: 0, rightBicepMeasure: 0,
      leftCuadricepsMeasure: 0, rightCuadricepsMeasure: 0
    },
    useAiAdjustment: false
  };

  showOptionalMeasures: boolean = false;
  isLoading: boolean = false;
  errorMessage: string = '';
  successMessage: string = '';
  aiFeedback: string = '';

  constructor(
    private reportService: WeekReportService,
    private userService: UserService,
    private router: Router,
    private cdr: ChangeDetectorRef,
    private ngZone: NgZone
  ) {}

  ngOnInit(): void {
    const userId = this.userService.getUserId();
    if (!userId) {
      this.router.navigate(['/login']);
      return;
    }
    this.report.idUser = userId;
  }

  goHome(): void {
    this.router.navigate(['/home']);
  }

  toggleMeasures(): void {
    this.showOptionalMeasures = !this.showOptionalMeasures;
  }

  goToStep2(): void {
    this.step = 2;
    window.scrollTo({ top: 0, behavior: 'smooth' });
  }

  backToStep1(): void {
    this.step = 1;
  }

  generatePlan(useAi: boolean): void {
    if (this.isLoading) return;

    this.isLoading = true;
    this.errorMessage = '';
    this.cdr.detectChanges();

    const cleanPayload = {
      idUser: this.report.idUser,
      hungerLevel: Number(this.report.hungerLevel),
      sleepQuality: Number(this.report.sleepQuality),
      energyLevel: Number(this.report.energyLevel),
      dietAdherence: Number(this.report.dietAdherence),
      description: this.report.description || "",
      useAiAdjustment: useAi,
      newWeight: (this.showOptionalMeasures && this.report.newWeight) ? Number(this.report.newWeight) : null,
      newMeasures: this.showOptionalMeasures ? {
        chestMeasure: Number(this.report.newMeasures.chestMeasure) || 0,
        waistMeasure: Number(this.report.newMeasures.waistMeasure) || 0,
        hipMeasure: Number(this.report.newMeasures.hipMeasure) || 0,
        leftBicepMeasure: Number(this.report.newMeasures.leftBicepMeasure) || 0,
        rightBicepMeasure: Number(this.report.newMeasures.rightBicepMeasure) || 0,
        leftCuadricepsMeasure: Number(this.report.newMeasures.leftCuadricepsMeasure) || 0,
        rightCuadricepsMeasure: Number(this.report.newMeasures.rightCuadricepsMeasure) || 0
      } : null
    };

    console.log(" Enviando petición...", cleanPayload);

    this.reportService.submitReport(cleanPayload).subscribe({
      next: (res: any) => {
        console.log(" RESPUESTA OK", res);
        
        this.ngZone.run(() => {
          this.aiFeedback = res?.aiAnalysis || res?.AiAnalysis || res?.analysisMessage || '';
          this.isLoading = false;
          this.step = 3; 
          
          this.cdr.markForCheck(); 
          this.cdr.detectChanges(); 
        });
      },
      error: (err) => {
        console.error(" ERROR", err);
        
        this.ngZone.run(() => {
          this.isLoading = false;
          this.errorMessage = "Hubo un error al generar el plan. Inténtalo de nuevo.";
          this.cdr.detectChanges();
        });
      }
    });
  }
}