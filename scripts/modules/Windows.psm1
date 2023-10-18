# https://learn.microsoft.com/pl-pl/windows/win32/api/winbase/nf-winbase-setthreadexecutionstate

$code=@' 
[DllImport("kernel32.dll", CharSet = CharSet.Auto,SetLastError = true)]
public static extern void SetThreadExecutionState(uint esFlags);
'@

$ste = Add-Type -MemberDefinition $code -Name System -Namespace Win32 -PassThru

Set-Variable -name ES_CONTINUOUS -value $([uint32]"0x80000000") -Scope Global -Option ReadOnly -Force
Set-Variable -name ES_AWAYMODE_REQUIRED -value $([uint32]"0x00000040") -Scope Global -Option ReadOnly -Force
Set-Variable -name ES_DISPLAY_REQUIRED -value $([uint32]"0x00000002") -Scope Global -Option ReadOnly -Force
Set-Variable -name ES_SYSTEM_REQUIRED -value $([uint32]"0x00000001") -Scope Global -Option ReadOnly -Force

function Set-ThreadExecutionState([uint32]$esFlags) {

    $ste::SetThreadExecutionState($esFlags)
}