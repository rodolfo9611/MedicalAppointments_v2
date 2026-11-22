# Script para iniciar todos los servicios del proyecto MediCare Hub
Write-Host "==========================================================" -ForegroundColor Cyan
Write-Host "  Iniciando Ecosistema MediCare Hub (UPES 2026)" -ForegroundColor Cyan
Write-Host "==========================================================" -ForegroundColor Cyan

$root = $PSScriptRoot

# 1. Microservicio de Citas (Puerto 5000)
Write-Host "`n[1/3] Iniciando Microservicio de Citas (http://localhost:5000)..." -ForegroundColor Yellow
Start-Process powershell -ArgumentList "-NoExit", "-Command", "cd '$root\src\MedicalAppointments.Api'; dotnet run --launch-profile http"

Start-Sleep -Seconds 3

# 2. Microservicio de Notificaciones & Worker Service (Puerto 5100)
Write-Host "[2/3] Iniciando Microservicio de Notificaciones & Worker Service (http://localhost:5100)..." -ForegroundColor Yellow
Start-Process powershell -ArgumentList "-NoExit", "-Command", "cd '$root\src\Notifications\MedicalAppointments.Notifications.Api'; dotnet run --launch-profile http"

Start-Sleep -Seconds 3

# 3. Frontend en Angular 20 (Puerto 4200)
Write-Host "[3/3] Iniciando Frontend Angular (http://localhost:4200)..." -ForegroundColor Yellow
Start-Process powershell -ArgumentList "-NoExit", "-Command", "cd '$root\frontend'; ng serve --open"

Write-Host "`n==========================================================" -ForegroundColor Green
Write-Host "  ¡Todos los servicios han sido lanzados!" -ForegroundColor Green
Write-Host "  - Swagger Citas:            http://localhost:5000/swagger" -ForegroundColor White
Write-Host "  - Swagger Notificaciones:   http://localhost:5100/swagger" -ForegroundColor White
Write-Host "  - Frontend Angular:         http://localhost:4200" -ForegroundColor White
Write-Host "==========================================================" -ForegroundColor Green
