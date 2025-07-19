# Monitoring & Alerting System

Ein System zur Überwachung von Servern und Diensten.

## Komponenten

- **Monitor Agents**: Sammeln Metriken von Servern
- **Alert Manager**: Verarbeitet Warnungen
- **Dashboard**: Zeigt Status in Echtzeit

## Schnellstart

1. Server starten:
   ```bash
   LocalNetAppChat.Server --port 5000 --key "monitor123"
   ```

2. Monitor Agent (auf jedem zu überwachenden Server):
   ```bash
   LocalNetAppChat.ConsoleClient emitter --server monitorserver --port 5000 --key "monitor123" --clientName "Server01" --command "python -u scripts/collect-metrics.py"
   ```

3. Alert Manager:
   ```bash
   LocalNetAppChat.Bot --server monitorserver --port 5000 --key "monitor123" --clientName "AlertManager" --scriptspath "./scripts"
   ```

4. Dashboard (Listener):
   ```bash
   LocalNetAppChat.ConsoleClient listener --server monitorserver --port 5000 --key "monitor123" --clientName "Dashboard"
   ```

## Scripts

### scripts/collect-metrics.py
Sammelt CPU, Memory, Disk-Metriken.

### scripts/check-alert.ps1
Prüft Metriken und löst Alerts aus.

### scripts/send-notification.ps1
Sendet Benachrichtigungen bei kritischen Events.