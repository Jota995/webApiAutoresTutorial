using System.ComponentModel.DataAnnotations;
using WebApiAutores.Validaciones;
using Xunit.Sdk;

namespace WebApiAutores.Test.UnitTest
{
    [TestClass]
    public class UppercaseAttributeTest
    {
        [TestMethod]
        public void PrimeraLetraMinuscula_DevuelveError()
        {
            var primeraLetraMayuscula = new UppercaseAttribute();
            var valor = "jose";
            var valContext = new ValidationContext(new {Nombre = valor});

            var resultado = primeraLetraMayuscula.GetValidationResult(valor, valContext);

            Assert.AreEqual("La primera letra debe ser mayuscula", resultado.ErrorMessage);
        }

        [TestMethod]
        public void ValorNulo_NoDevuelveNulo()
        {
            var primeraLetraMayuscula = new UppercaseAttribute();
            string valor = null;
            var valContext = new ValidationContext(new { Nombre = valor });

            var resultado = primeraLetraMayuscula.GetValidationResult(valor, valContext);

            Assert.IsNull(resultado);
        }

        [TestMethod]
        public void ValorConPrimeraLetraMayuscula_NoDevuelveError ()
        {
            var primeraLetraMayuscula = new UppercaseAttribute();
            string valor = "Jose";
            var valContext = new ValidationContext(new { Nombre = valor });

            var resultado = primeraLetraMayuscula.GetValidationResult(valor, valContext);

            Assert.IsNull(resultado);
        }
    }
}