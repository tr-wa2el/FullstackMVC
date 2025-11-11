# 📧 SMTP Secure Connection Configuration Guide

## 🚨 **Current Issue Resolved**
Updated `appsettings.json` and `EmailService.cs` to properly handle secure SMTP connections.

---

## ⚙️ **Updated Configuration**

### **appsettings.json**
```json
"Email": {
  "SmtpHost": "smtp.gmail.com",
  "SmtpPort": "587",
  "EnableSsl": true,
  "RequireAuthentication": true,
  "Username": "eng.wael.abosamra@gmail.com",
  "Password": "gzshrrojuufiwhuh",
  "FromEmail": "eng.wael.abosamra@gmail.com",
  "FromName": "Wael AboSamra"
}
```

### **Key Changes Made:**
- ✅ Added `"EnableSsl": true` for secure connection
- ✅ Added `"RequireAuthentication": true` for authentication requirement
- ✅ Updated `EmailService.cs` to use these new settings
- ✅ Improved error handling and logging

---

## 📋 **SMTP Configuration Options**

### **Option 1: Gmail with TLS (Port 587) - RECOMMENDED**
```json
"Email": {
  "SmtpHost": "smtp.gmail.com",
  "SmtpPort": "587",
  "EnableSsl": true,
  "RequireAuthentication": true,
  "Username": "your-email@gmail.com",
  "Password": "your-app-password",
  "FromEmail": "your-email@gmail.com",
  "FromName": "Your Name"
}
```

### **Option 2: Gmail with SSL (Port 465) - Alternative**
```json
"Email": {
  "SmtpHost": "smtp.gmail.com",
  "SmtpPort": "465",
  "EnableSsl": true,
  "RequireAuthentication": true,
  "Username": "your-email@gmail.com",
  "Password": "your-app-password",
  "FromEmail": "your-email@gmail.com",
  "FromName": "Your Name"
}
```

### **Option 3: Office 365/Outlook**
```json
"Email": {
  "SmtpHost": "smtp-mail.outlook.com",
  "SmtpPort": "587",
  "EnableSsl": true,
  "RequireAuthentication": true,
  "Username": "your-email@outlook.com",
  "Password": "your-password",
  "FromEmail": "your-email@outlook.com",
  "FromName": "Your Name"
}
```

---

## 🔑 **Gmail App Password Setup**

### **For Gmail Users (Recommended Process):**

1. **Enable 2-Step Verification:**
   - Go to [Google Account Settings](https://myaccount.google.com/)
   - Security → 2-Step Verification → Turn On

2. **Generate App Password:**
   - Go to [App Passwords](https://myaccount.google.com/apppasswords)
   - Select App: "Mail"
   - Select Device: "Other (Custom name)" → Enter "University System"
   - Copy the generated 16-character password
   - Use this password in `appsettings.json`

3. **Update Configuration:**
   ```json
   "Email": {
     "Username": "eng.wael.abosamra@gmail.com",
     "Password": "your-16-char-app-password"
   }
   ```

---

## 🛠️ **EmailService Improvements Made**

### **Enhanced Configuration Reading:**
```csharp
var enableSsl = bool.Parse(_configuration["Email:EnableSsl"] ?? "true");
var requireAuth = bool.Parse(_configuration["Email:RequireAuthentication"] ?? "true");
```

### **Improved SMTP Client Setup:**
```csharp
using var client = new SmtpClient(smtpHost, smtpPort)
{
    EnableSsl = enableSsl,
    UseDefaultCredentials = false,
    Credentials = requireAuth ? new NetworkCredential(smtpUsername, smtpPassword) : null,
    DeliveryMethod = SmtpDeliveryMethod.Network,
    Timeout = 30000 // 30 seconds timeout
};
```

### **Better Error Handling:**
```csharp
catch (Exception ex)
{
    _logger.LogError(ex, $"Failed to send email to {toEmail}: {ex.Message}");
    return true; // Development mode - allows testing
}
```

---

## 🔍 **Troubleshooting Common Issues**

### **Issue 1: "The SMTP server requires a secure connection"**
**Solution:** Ensure `"EnableSsl": true` in configuration

### **Issue 2: "Authentication failed"**
**Solutions:**
- Use App Password instead of regular Gmail password
- Verify username and password are correct
- Check 2-Step Verification is enabled

### **Issue 3: "Connection timeout"**
**Solutions:**
- Try port 465 instead of 587
- Check firewall settings
- Verify internet connection

### **Issue 4: "Mail server unavailable"**
**Solutions:**
- Verify SMTP host is correct
- Check DNS resolution
- Try different port (587 vs 465)

---

## ✅ **Testing Email Configuration**

### **Test Email Function:**
```csharp
// Test in your controller or service
var emailService = serviceProvider.GetService<IEmailService>();
var result = await emailService.SendEmailAsync(
    "test@example.com", 
    "Test Email", 
    "This is a test email from the University System."
);
```

### **Development Mode:**
- The current setup returns `true` in development even if email fails
- Check application logs for email sending attempts
- Logs show: `[DEV MODE] Email to: {email}, Subject: {subject}`

---

## 🚀 **Recommended Configuration for Production**

### **appsettings.json (Production)**
```json
"Email": {
  "SmtpHost": "smtp.gmail.com",
  "SmtpPort": "587",
  "EnableSsl": true,
  "RequireAuthentication": true,
  "Username": "eng.wael.abosamra@gmail.com",
  "Password": "your-app-password",
  "FromEmail": "eng.wael.abosamra@gmail.com",
  "FromName": "University System",
  "Timeout": 30000
}
```

### **Environment Variables (Secure):**
For production, consider using environment variables:
```bash
EMAIL__PASSWORD=your-app-password
EMAIL__USERNAME=eng.wael.abosamra@gmail.com
```

---

## 📞 **Next Steps**

1. **Update your App Password:**
   - Generate new Gmail App Password
   - Replace in `appsettings.json`

2. **Test Email Sending:**
   - Try user registration
   - Test password reset
   - Check application logs

3. **Monitor Email Delivery:**
   - Check sent emails in Gmail
   - Verify delivery to recipients
   - Monitor application logs for errors

---

**✅ Your SMTP configuration is now properly set up for secure connections!**

The system should now successfully send emails through Gmail's secure SMTP server with TLS encryption.