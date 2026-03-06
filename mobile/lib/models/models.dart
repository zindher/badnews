class UserModel {
  final String id;
  final String name;
  final String email;
  final String? avatarUrl;
  final double rating;
  final int completedOrders;

  UserModel({
    required this.id,
    required this.name,
    required this.email,
    this.avatarUrl,
    required this.rating,
    required this.completedOrders,
  });

  factory UserModel.fromJson(Map<String, dynamic> json) {
    final firstName = json['firstName'] ?? '';
    final lastName = json['lastName'] ?? '';
    return UserModel(
      id: json['id'] ?? '',
      name: '$firstName $lastName'.trim().isEmpty ? (json['name'] ?? '') : '$firstName $lastName'.trim(),
      email: json['email'] ?? '',
      avatarUrl: json['googleProfilePictureUrl'] ?? json['avatarUrl'],
      rating: (json['averageRating'] as num?)?.toDouble() ?? 0.0,
      completedOrders: json['totalCompletedOrders'] ?? 0,
    );
  }
}

class RecordingModel {
  final String id;
  final int duration;
  final String createdAt;
  final String? messengerName;
  final String? url;

  RecordingModel({
    required this.id,
    required this.duration,
    required this.createdAt,
    this.messengerName,
    this.url,
  });

  factory RecordingModel.fromJson(Map<String, dynamic> json) {
    return RecordingModel(
      id: json['id'] ?? '',
      duration: json['duration'] ?? 0,
      createdAt: json['createdAt'] ?? '',
      messengerName: json['messengerName'],
      url: json['url'] ?? json['recordingUrl'],
    );
  }
}

class ChatMessage {
  final String id;
  final String senderId;
  final String content;
  final String timestamp;

  ChatMessage({
    required this.id,
    required this.senderId,
    required this.content,
    required this.timestamp,
  });

  factory ChatMessage.fromJson(Map<String, dynamic> json) {
    return ChatMessage(
      id: json['id'] ?? '',
      senderId: json['senderId'] ?? '',
      content: json['content'] ?? '',
      timestamp: json['createdAt'] ?? json['timestamp'] ?? '',
    );
  }
}

class TransactionModel {
  final String orderId;
  final String date;
  final double amount;

  TransactionModel({
    required this.orderId,
    required this.date,
    required this.amount,
  });

  factory TransactionModel.fromJson(Map<String, dynamic> json) {
    return TransactionModel(
      orderId: json['orderId'] ?? '',
      date: json['createdAt'] ?? json['date'] ?? '',
      amount: (json['amount'] as num?)?.toDouble() ?? 0.0,
    );
  }
}

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
      price: (json['price'] as num?)?.toDouble() ?? 0.0,
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
      averageRating: (json['averageRating'] as num?)?.toDouble() ?? 0.0,
      totalCompletedOrders: json['totalCompletedOrders'] ?? 0,
      totalEarnings: (json['totalEarnings'] as num?)?.toDouble() ?? 0.0,
      pendingBalance: (json['pendingBalance'] as num?)?.toDouble() ?? 0.0,
    );
  }
}
