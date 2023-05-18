using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public enum CorTinta // brainstorming
{
    Vermelho,
    Verde,
    Azul,
}

public static class Core
{
    // tinta
    private static int quantidadeTinta = 0;
    public static int GetQuantidadeTinta()
    {
        return quantidadeTinta;
    }
    public static void IncrementaQuantidadeTinta(int incremento)
    {
        quantidadeTinta += incremento;
        UpdateNumeroTinta();
    }
    public static void SetQuantidadeTinta(int novaQuantidade)
    {
        quantidadeTinta = novaQuantidade;
        UpdateNumeroTinta();
    }

    public static bool SetIndicadorGrafite(bool indicadorSetActive)
    {
        GameObject indicadorGrafite = Camera.main.transform.GetChild(0).GetChild(1).gameObject; // pessimo, mudar dps
        indicadorGrafite.SetActive(indicadorSetActive);
        return true;
    }

    private static void UpdateNumeroTinta()
    {
        Transform referenciaUI = Camera.main.transform.GetChild(0);
        if(referenciaUI != null)
        {
            referenciaUI.GetComponentInChildren<TextMeshProUGUI>().text = "TINTA: " + quantidadeTinta.ToString();
        }
    }

    // ruas
    /* NAO ESTA SENDO USADA AINDA (só no "menu") */
    private static string UltimaRuaVisitada = "MainMenu";
    private static string RuaAtual = "MainMenu";

    public static void MudarRua(string NomeRua)
    {
        UltimaRuaVisitada = RuaAtual;
        RuaAtual = NomeRua;
        SceneManager.LoadScene(NomeRua);
    }

    public static void Reset()
    {
        quantidadeTinta = 0;
    }
}
