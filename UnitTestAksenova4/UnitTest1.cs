using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using Практическая_работа_4_Солодовников_Кураев.Pages;

namespace UnitTestAksenova4
{
    [TestClass]
    public class AuthTests
    {
        private AuthPage _authPage;
        private RegPage _regPage;
        private Dictionary<string, string> _testUsers;

        [TestInitialize]
        public void Setup()
        {
            _testUsers = new Dictionary<string, string>
            {
                { "admin", "123456" },
                { "user1", "pass1" }
            };

            _authPage = new AuthPage();
            _regPage = new RegPage(_testUsers);
        }

        [TestMethod]
        public void AuthTest_ValidCredentials_ReturnsTrue()
        {
            bool result = _authPage.Auth("admin", "123456");
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void AuthTest_InvalidPassword_ReturnsFalse()
        {
            bool result = _authPage.Auth("admin", "wrongpassword");
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void AuthTest_NonExistentUser_ReturnsFalse()
        {
            bool result = _authPage.Auth("nonexistent", "123456");
            Assert.IsFalse(result);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void AuthTest_EmptyLogin_ThrowsException()
        {
            _authPage.Auth("", "123456");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void AuthTest_EmptyPassword_ThrowsException()
        {
            _authPage.Auth("admin", "");
        }

        [TestMethod]
        public void AuthTest_Success_AllValidUsers()
        {
            var validUsers = new Dictionary<string, string>
            {
                { "admin", "123456" },
                { "user1", "pass1" }
            };

            foreach (var user in validUsers)
            {
                Assert.IsTrue(_authPage.Auth(user.Key, user.Value));
            }
        }

        [TestMethod]
        public void AuthTest_Fail_InvalidCredentials()
        {
            var invalidCredentials = new[]
            {
                new { Login = "admin", Password = "wrong" },
                new { Login = "user1", Password = "wrongpass" },
                new { Login = "nonexistent", Password = "123" }
            };

            foreach (var cred in invalidCredentials)
            {
                Assert.IsFalse(_authPage.Auth(cred.Login, cred.Password));
            }
        }

        [TestMethod]
        public void RegisterTest_ValidData_ReturnsTrue()
        {
            bool result = _regPage.Register("newuser", "password123", "password123");
            Assert.IsTrue(result);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void RegisterTest_EmptyLogin_ThrowsException()
        {
            _regPage.Register("", "pass123", "pass123");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void RegisterTest_EmptyPassword_ThrowsException()
        {
            _regPage.Register("newuser", "", "");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void RegisterTest_PasswordTooShort_ThrowsException()
        {
            _regPage.Register("newuser", "123", "123");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void RegisterTest_PasswordsDoNotMatch_ThrowsException()
        {
            _regPage.Register("newuser", "password123", "different123");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void RegisterTest_UserAlreadyExists_ThrowsException()
        {
            _regPage.Register("admin", "123456", "123456");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void RegisterTest_InvalidLoginCharacters_ThrowsException()
        {
            _regPage.Register("user name!", "password123", "password123");
        }

        [TestMethod]
        public void CaptchaTest_ActivatesAfterThreeFailedAttempts()
        {
            _authPage.ResetFailedAttempts();

            for (int i = 0; i < 2; i++)
            {
                try { _authPage.AuthWithAttempts("admin", "wrong"); }
                catch { }
                Assert.IsFalse(_authPage.NeedCaptcha(), $"После {i + 1} попытки CAPTCHA не требуется");
            }

            try { _authPage.AuthWithAttempts("admin", "wrong"); }
            catch { }
            Assert.IsTrue(_authPage.NeedCaptcha(), "После 3 неудачных попыток CAPTCHA требуется");
            Assert.AreEqual(3, _authPage.GetFailedAttempts(), "Счетчик должен показывать 3");
        }

        [TestMethod]
        public void CaptchaTest_ResetsAfterSuccessfulLogin()
        {
            _authPage.ResetFailedAttempts();

            for (int i = 0; i < 3; i++)
            {
                try { _authPage.AuthWithAttempts("admin", "wrong"); }
                catch { }
            }
            Assert.IsTrue(_authPage.NeedCaptcha(), "CAPTCHA активна");

            string captchaCode = _authPage.GetCurrentCaptchaCode();
            bool result = _authPage.AuthWithAttempts("admin", "123456", captchaCode);

            Assert.IsTrue(result, "Успешный вход");
            Assert.IsFalse(_authPage.NeedCaptcha(), "CAPTCHA не требуется");
            Assert.AreEqual(0, _authPage.GetFailedAttempts(), "Счетчик сброшен");
        }

        [TestMethod]
        public void CaptchaTest_ValidCaptchaAllowsLogin()
        {
            _authPage.ResetFailedAttempts();
            for (int i = 0; i < 3; i++)
            {
                try { _authPage.AuthWithAttempts("admin", "wrong"); }
                catch { }
            }

            string captchaCode = _authPage.GetCurrentCaptchaCode();
            bool result = _authPage.AuthWithAttempts("admin", "123456", captchaCode);

            Assert.IsTrue(result, "Правильный CAPTCHA должен пропускать");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void CaptchaTest_InvalidCaptchaThrowsException()
        {
            _authPage.ResetFailedAttempts();

            for (int i = 0; i < 3; i++)
            {
                try { _authPage.AuthWithAttempts("admin", "wrong"); }
                catch { }
            }

            _authPage.AuthWithAttempts("admin", "123456", "WRONG_CODE");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void CaptchaTest_EmptyCaptchaThrowsException()
        {
            _authPage.ResetFailedAttempts();

            for (int i = 0; i < 3; i++)
            {
                try { _authPage.AuthWithAttempts("admin", "wrong"); }
                catch { }
            }

            _authPage.AuthWithAttempts("admin", "123456", "");
        }

        [TestMethod]
        public void CaptchaTest_CodeHasValidFormat()
        {
            _authPage.ResetFailedAttempts();

            for (int i = 0; i < 3; i++)
            {
                try { _authPage.AuthWithAttempts("admin", "wrong"); }
                catch { }
            }

            string code = _authPage.GetCurrentCaptchaCode();
            string allowedChars = "ABCDEFGHJKLMNPQRSTUVWXYZ23456789";

            Assert.IsNotNull(code);
            Assert.IsTrue(code.Length >= 4 && code.Length <= 6, "Длина 4-6 символов");

            foreach (char c in code)
            {
                Assert.IsTrue(allowedChars.Contains(c), $"Символ '{c}' недопустим");
            }
        }

        [TestMethod]
        public void CaptchaTest_RefreshGeneratesNewCode()
        {
            _authPage.ResetFailedAttempts();

            for (int i = 0; i < 3; i++)
            {
                try { _authPage.AuthWithAttempts("admin", "wrong"); }
                catch { }
            }

            string oldCode = _authPage.GetCurrentCaptchaCode();
            _authPage.RefreshCaptcha();
            string newCode = _authPage.GetCurrentCaptchaCode();

            Assert.AreNotEqual(oldCode, newCode, "Код должен измениться");
        }

        [TestMethod]
        public void CaptchaTest_CounterIncreasesOnFailedAttempts()
        {
            _authPage.ResetFailedAttempts();
            Assert.AreEqual(0, _authPage.GetFailedAttempts());

            try { _authPage.AuthWithAttempts("admin", "wrong"); }
            catch { }
            Assert.AreEqual(1, _authPage.GetFailedAttempts());

            try { _authPage.AuthWithAttempts("admin", "wrong"); }
            catch { }
            Assert.AreEqual(2, _authPage.GetFailedAttempts());

            try { _authPage.AuthWithAttempts("admin", "wrong"); }
            catch { }
            Assert.AreEqual(3, _authPage.GetFailedAttempts());
            Assert.IsTrue(_authPage.NeedCaptcha());
        }

        [TestMethod]
        public void CaptchaTest_CounterDoesNotIncreaseOnValidCaptcha()
        {
            _authPage.ResetFailedAttempts();

            for (int i = 0; i < 3; i++)
            {
                try { _authPage.AuthWithAttempts("admin", "wrong"); }
                catch { }
            }

            int beforeCount = _authPage.GetFailedAttempts();
            string captchaCode = _authPage.GetCurrentCaptchaCode();
            bool result = _authPage.AuthWithAttempts("admin", "123456", captchaCode);

            Assert.IsTrue(result);
            Assert.AreEqual(0, _authPage.GetFailedAttempts(), "Счетчик должен сброситься, а не увеличиться");
        }
    }
}