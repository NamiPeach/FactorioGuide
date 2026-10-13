using System.ComponentModel.DataAnnotations;
using Factorio.Domain.Enums;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ServerApp.Models;

/// <summary>Form model for creating and editing an action.</summary>
public class ActionFormModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Choose a user.")]
    [Display(Name = "User")]
    public int? UserId { get; set; }

    [Required(ErrorMessage = "Choose an item.")]
    [Display(Name = "Item")]
    public int? ItemId { get; set; }

    public ActionType Type { get; set; } = ActionType.Edit;

    [StringLength(2000)]
    [Display(Name = "Proposed description (empty for Delete)")]
    public string? Details { get; set; }

    public List<SelectListItem> UserOptions { get; set; } = new();
    public List<SelectListItem> ItemOptions { get; set; } = new();
}
