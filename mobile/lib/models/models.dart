class OrderModel {
  final String id;
  final String recipientName;
  final String recipientPhone;
  final String message;
  final bool isAnonymous;
  final double price;
  final String status;
  final DateTime createdAt;
  final String? buyerName;

  OrderModel({
    required this.id,
    required this.recipientName,
    required this.recipientPhone,
    required this.message,
    required this.isAnonymous,
    required this.price,
    required this.status,
    required this.createdAt,
    this.buyerName,
  });

  factory OrderModel.fromJson(Map<String, dynamic> json) {
    return OrderModel(
      id: json['id'] ?? '',
      recipientName: json['recipientName'] ?? '',
      recipientPhone: json['recipientPhone'] ?? '',
      message: json['message'] ?? '',
      isAnonymous: json['isAnonymous'] ?? false,
      price: (json['price'] as num).toDouble(),
      status: json['status'] ?? 'Pending',
      createdAt: DateTime.parse(json['createdAt'] ?? DateTime.now().toString()),
      buyerName: json['buyerName'],
    );
  }

  Map<String, dynamic> toJson() => {
    'id': id,
    'recipientName': recipientName,
    'recipientPhone': recipientPhone,
    'message': message,
    'isAnonymous': isAnonymous,
    'price': price,
    'status': status,
    'createdAt': createdAt.toIso8601String(),
    'buyerName': buyerName,
  };
}

class MessengerModel {
  final String id;
  final String userId;
  final bool isAvailable;
  final double averageRating;
  final int totalCompletedOrders;
  final double totalEarnings;
  final double pendingBalance;

  MessengerModel({
    required this.id,
    required this.userId,
    required this.isAvailable,
    required this.averageRating,
    required this.totalCompletedOrders,
    required this.totalEarnings,
    required this.pendingBalance,
  });

  factory MessengerModel.fromJson(Map<String, dynamic> json) {
    return MessengerModel(
      id: json['id'] ?? '',
      userId: json['userId'] ?? '',
      isAvailable: json['isAvailable'] ?? false,
      averageRating: (json['averageRating'] as num).toDouble(),
      totalCompletedOrders: json['totalCompletedOrders'] ?? 0,
      totalEarnings: (json['totalEarnings'] as num).toDouble(),
      pendingBalance: (json['pendingBalance'] as num).toDouble(),
    );
  }
}
