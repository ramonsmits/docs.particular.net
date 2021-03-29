while($true)
{
    $i++
    Write-Host "Sleeping 5 minutes"
    Start-Sleep -s 300

    Write-Host "Killing #$i"
    docker kill servicecontrol_4_error_1
    docker kill servicecontrol_4_audit_1

    Write-Host "Starting"
    docker-compose up --detach
}
