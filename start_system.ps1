# Script para iniciar todos los servicios del proyecto MediCare Hub
Write-Host "==========================================================" -ForegroundColor Cyan
Write-Host "  Iniciando Ecosistema MediCare Hub (UPES 2026)" -ForegroundColor Cyan
Write-Host "==========================================================" -ForegroundColor Cyan

$root = $PSScriptRoot

# 1. Microservicio de Citas (Puerto 5000)
Write-Host "`n[1/6] Iniciando Microservicio de Citas (http://localhost:5000)..." -ForegroundColor Yellow
Start-Process powershell -ArgumentList "-NoExit", "-Command", "cd '$root\src\MedicalAppointments.Api'; dotnet run --launch-profile http"

Start-Sleep -Seconds 3

# 2. Microservicio de Notificaciones & Worker Service (Puerto 5100)
Write-Host "[2/6] Iniciando Microservicio de Notificaciones & Worker Service (http://localhost:5100)..." -ForegroundColor Yellow
Start-Process powershell -ArgumentList "-NoExit", "-Command", "cd '$root\src\Notifications\MedicalAppointments.Notifications.Api'; dotnet run --launch-profile http"

Start-Sleep -Seconds 3

# 3. Microservicio de Pacientes (Puerto 5015)
Write-Host "[3/6] Iniciando Microservicio de Pacientes (http://localhost:5015)..." -ForegroundColor Yellow
Start-Process powershell -ArgumentList "-NoExit", "-Command", "cd '$root\MicroservicioPacientes.Api'; dotnet run --launch-profile http"

Start-Sleep -Seconds 3

# 4. Microservicio de Recetas Médicas (Puerto 5200)
Write-Host "[4/6] Iniciando Microservicio de Recetas Médicas (http://localhost:5200)..." -ForegroundColor Yellow
Start-Process powershell -ArgumentList "-NoExit", "-Command", "cd '$root\src\RecetasMedicas\MedicalAppointments.RecetasMedicas.Api'; dotnet run --launch-profile http"

Start-Sleep -Seconds 3

# 5. Microservicio de Reportes y Dashboard (Puerto 5300)
Write-Host "[5/6] Iniciando Microservicio de Reportes y Dashboard (http://localhost:5300)..." -ForegroundColor Yellow
Start-Process powershell -ArgumentList "-NoExit", "-Command", "cd '$root\src\Reportes\MedicalAppointments.Reportes.Api'; dotnet run --launch-profile http"

Start-Sleep -Seconds 3

# 6. Frontend en Angular 20 (Puerto 4200)
Write-Host "[6/6] Iniciando Frontend Angular (http://localhost:4200)..." -ForegroundColor Yellow
Start-Process powershell -ArgumentList "-NoExit", "-Command", "cd '$root\frontend'; ng serve --open"

Write-Host "`n==========================================================" -ForegroundColor Green
Write-Host "  ¡Todos los servicios han sido lanzados!" -ForegroundColor Green
Write-Host "  - Swagger Citas:            http://localhost:5000/swagger" -ForegroundColor White
Write-Host "  - Swagger Notificaciones:   http://localhost:5100/swagger" -ForegroundColor White
Write-Host "  - Swagger Pacientes:        http://localhost:5015/swagger" -ForegroundColor White
Write-Host "  - Swagger Recetas:          http://localhost:5200/swagger" -ForegroundColor White
Write-Host "  - Swagger Reportes:         http://localhost:5300/swagger" -ForegroundColor White
Write-Host "  - Frontend Angular:         http://localhost:4200" -ForegroundColor White
Write-Host "==========================================================" -ForegroundColor Green
