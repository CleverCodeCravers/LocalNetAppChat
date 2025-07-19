param([int]$number)

$celebrations = @(
    "Jippeah! $number ist eine grosse Zahl!",
    "Fantastisch! $number uebersteigt die 10!",
    "Wow! $number - Das ist beeindruckend!",
    "Hurra! $number ist im zweistelligen Bereich!",
    "Grossartig! $number ist mehr als 10!"
)

$message = $celebrations | Get-Random
Write-Output $message

# Spezial-Reaktion bei sehr grossen Zahlen
if ($number -ge 18) {
    Write-Output "MEGA! Das ist fast das Maximum!"
}